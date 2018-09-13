/****************************************************************************
** Copyright (c) 2013-2018 Mazatech S.r.l.
** All rights reserved.
** 
** Redistribution and use in source and binary forms, with or without
** modification, are permitted (subject to the limitations in the disclaimer
** below) provided that the following conditions are met:
** 
** - Redistributions of source code must retain the above copyright notice,
**   this list of conditions and the following disclaimer.
** 
** - Redistributions in binary form must reproduce the above copyright notice,
**   this list of conditions and the following disclaimer in the documentation
**   and/or other materials provided with the distribution.
** 
** - Neither the name of Mazatech S.r.l. nor the names of its contributors
**   may be used to endorse or promote products derived from this software
**   without specific prior written permission.
** 
** NO EXPRESS OR IMPLIED LICENSES TO ANY PARTY'S PATENT RIGHTS ARE GRANTED
** BY THIS LICENSE. THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
** CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT
** NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
** A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER
** OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
** EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
** PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
** OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
** WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
** OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
** ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
** 
** For any information, please contact info@mazatech.com
** 
****************************************************************************/
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SVGAtlasSelector : ScriptableWizard
{
    static private List<SVGAtlas> GetAtlasesList()
    {
        string[] guids = AssetDatabase.FindAssets("t:SVGAtlas");
        List<SVGAtlas> atlasesList = new List<SVGAtlas>();

        foreach (string assetGUID in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(SVGAtlas));
            SVGAtlas atlas = asset as SVGAtlas;
            
            if (atlas != null)
                atlasesList.Add(atlas);
        }

        // sort atlases by name
        atlasesList.Sort(delegate(SVGAtlas atlas1, SVGAtlas atlas2) {
            return string.Compare(atlas1.name, atlas2.name, StringComparison.OrdinalIgnoreCase);
        });

        return atlasesList;
    }

    // retrieves a list of all atlas whose names contain the specified match string
    static private List<SVGAtlas> FilterAtlasesList(List<SVGAtlas> wholeList, string match)
    {
        List<SVGAtlas> result;

        if (wholeList == null)
            return new List<SVGAtlas>();

        if (string.IsNullOrEmpty(match))
            return wholeList;

        // create the output list
        result = new List<SVGAtlas>();

        // find an exact match
        foreach (SVGAtlas atlas in wholeList)
        {
            if (!string.IsNullOrEmpty(atlas.name) && string.Equals(match, atlas.name, StringComparison.OrdinalIgnoreCase))
                result.Add(atlas);
        }
        // if an exact match has been found, simply return the result
        if (result.Count > 0)
            return result;

        // search for (space) separated components
        string[] searchKeys = match.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < searchKeys.Length; ++i)
            searchKeys[i] = searchKeys[i].ToLower();

        // find all atlases whose names contain one ore more keyword
        foreach (SVGAtlas atlas in wholeList)
        {
            if (!string.IsNullOrEmpty(atlas.name))
            {
                string lowerName = atlas.name.ToLower();
                int matchesCount = 0;

                foreach (string key in searchKeys)
                {
                    if (lowerName.Contains(key))
                        matchesCount++;
                }

                // (matchesCount == searchKeys.Length) if we were interested in finding all atlases whose names contain ALL the keywords
                if (matchesCount > 0)
                    result.Add(atlas);
            }
        }

        return result;
    }

    private static void AtlasLabel(string atlasName, Rect rect)
    {
        GUI.backgroundColor = SVGAtlasSelector.ATLAS_NAME_BACKGROUND_COLOR;
        GUI.contentColor = SVGAtlasSelector.ATLAS_NAME_COLOR;
        // we use the atlas name as tooltip
        GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, SVGAtlasSelector.ATLAS_NAME_HEIGHT), new GUIContent(atlasName, atlasName), "ProgressBarBack");
        GUI.contentColor = Color.white;
        GUI.backgroundColor = Color.white;
    }

    // Draw an atlas preview
    private static void AtlasPreview(SVGAtlas atlas, Rect clipRect, int textureIndex)
    {
        int idx = textureIndex % atlas.TextureAssetsCount();
        AssetFile textureAsset = atlas.TextureAsset(idx);
        Texture2D atlasTexture = textureAsset.Object as Texture2D;

        if (atlasTexture != null)
        {
            Rect uv = new Rect(0, 0, 1, 1);
            float maxAtlasDim = Math.Max(atlasTexture.width, atlasTexture.height);
            float previewWidth = (atlasTexture.width / maxAtlasDim) * SVGAtlasSelector.ATLAS_PREVIEW_DIMENSION;
            float previewHeight = (atlasTexture.height / maxAtlasDim) * SVGAtlasSelector.ATLAS_PREVIEW_DIMENSION;
            float previewX = (SVGAtlasSelector.ATLAS_PREVIEW_DIMENSION - previewWidth) / 2;
            float previewY = (SVGAtlasSelector.ATLAS_PREVIEW_DIMENSION - previewHeight) / 2;
            Rect previewRect = new Rect(clipRect.xMin + previewX, clipRect.yMin + previewY, previewWidth, previewHeight);
            GUI.DrawTextureWithTexCoords(previewRect, atlasTexture, uv, true);
        }
    }

    private List<SVGAtlas> Header()
    {
        EditorGUIUtility.labelWidth = SVGAtlasSelector.ATLAS_PREVIEW_DIMENSION;
        // show atlas name
        GUILayout.Label("Available atlases", "LODLevelNotifyText");
        // the search toolbox
        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(85);
            this.m_SearchString = EditorGUILayout.TextField("", this.m_SearchString, "SearchTextField");
            if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18)))
            {
                this.m_SearchString = "";
                GUIUtility.keyboardControl = 0;
            }
            GUILayout.Space(85);
        }
        GUILayout.EndHorizontal();
        // return the filtered atlases list
        return SVGAtlasSelector.FilterAtlasesList(this.m_AtlasesList, this.m_SearchString);
    }

    private bool DrawGUI()
    {
        bool close = false;
        int columnsPerRow = Math.Max(Mathf.FloorToInt(Screen.width / SVGAtlasSelector.ATLAS_PREVIEW_DIMENSION_PADDED), 1);
        int rowsCount = 1;
        int atlasIdx = 0;
        Rect rect = new Rect(SVGAtlasSelector.ATLAS_PREVIEW_BORDER, SVGAtlasSelector.ATLAS_PREVIEW_BORDER,
                             SVGAtlasSelector.ATLAS_PREVIEW_DIMENSION, SVGAtlasSelector.ATLAS_PREVIEW_DIMENSION);
        
        // draw header, with the name of atlas and the "search by name" toolbox
        List<SVGAtlas> atlasesList = this.Header();
        //GUILayout.Space(10);

        this.m_ScrollPos = GUILayout.BeginScrollView(this.m_ScrollPos);
        while (atlasIdx < atlasesList.Count)
        {
            // start a new row
            GUILayout.BeginHorizontal();
            {
                int currentColumn = 0;
                rect.x = SVGAtlasSelector.ATLAS_PREVIEW_BORDER;
                
                while (atlasIdx < atlasesList.Count)
                {
                    SVGAtlas atlas = atlasesList[atlasIdx];

                    // buttons are used to implement atlas selection (we use the atlas name as tooltip)
                    if (GUI.Button(rect, new GUIContent("", atlas.name)))
                    {
                        // mouse left button click
                        if (Event.current.button == 0)
                        {
                            if (this.m_Callback != null)
                                this.m_Callback(atlas);
                            close = true;
                        }
                    }
                    // draw atlas preview
                    if (Event.current.type == EventType.Repaint)
                        SVGAtlasSelector.AtlasPreview(atlas, rect, this.m_TextureIndex);
                    // draw atlas name
                    SVGAtlasSelector.AtlasLabel(atlas.name, rect);
                    
                    // next atlas
                    atlasIdx++;
                    // next column
                    rect.x += SVGAtlasSelector.ATLAS_PREVIEW_DIMENSION_PADDED;
                    if (++currentColumn >= columnsPerRow)
                        break;
                }
            }
            
            GUILayout.EndHorizontal();
            GUILayout.Space(SVGAtlasSelector.ATLAS_PREVIEW_DIMENSION_PADDED);
            rect.y += SVGAtlasSelector.ATLAS_PREVIEW_DIMENSION_PADDED + SVGAtlasSelector.ATLAS_NAME_HEIGHT;
            rowsCount++;
        }
        
        GUILayout.Space((rowsCount - 1) * SVGAtlasSelector.ATLAS_NAME_HEIGHT + SVGAtlasSelector.ATLAS_PREVIEW_BORDER);
        GUILayout.EndScrollView();

        return close;
    }

    void OnGUI()
    {
        if (this.m_AtlasesList != null)
        {
            // draw the actual wizard content
            if (this.DrawGUI())
                this.Close();
        }
    }

    void OnEnable()
    {
        m_Instance = this;
    }

    void OnDisable()
    {
        m_Instance = null;
    }

    void Update()
    {
        // check time
        float currentTime = Time.realtimeSinceStartup;
        // advance to the next texture every 3 seconds
        if (currentTime > this.m_Time + 3)
        {
            this.m_Time = currentTime;
            this.m_TextureIndex++;
            this.Repaint();
        }
    }

    // show the atlas selector
    static public void Show(string atlasName, OnAtlasSelectionCallback callback)
    {
        // close the current selector instance, if any
        SVGAtlasSelector.CloseAll();

        SVGAtlasSelector selector = ScriptableWizard.DisplayWizard<SVGAtlasSelector>("Select an atlas");
        selector.m_SearchString = atlasName;
        selector.m_Callback = callback;
        selector.m_ScrollPos = Vector2.zero;
        selector.m_AtlasesList = SVGAtlasSelector.GetAtlasesList();
        selector.m_Time = Time.realtimeSinceStartup;
        selector.m_TextureIndex = 0;
    }

    static public void CloseAll()
    {
        // close the current selector instance, if any
        if (m_Instance != null)
        {
            m_Instance.Close();
            m_Instance = null;
        }
    }

    // Selection callback
    public delegate void OnAtlasSelectionCallback(SVGAtlas atlas);
    // The current selector instance
    [NonSerialized]
    static private SVGAtlasSelector m_Instance;
    // The string we are using for filtering names
    [NonSerialized]
    private string m_SearchString;
    // the whole atlases list
    [NonSerialized]
    List<SVGAtlas> m_AtlasesList;
    // The current scroll position
    [NonSerialized]
    private Vector2 m_ScrollPos;
    // The callback to be invoked when an atlas selection occurs
    [NonSerialized]
    private OnAtlasSelectionCallback m_Callback;
    // Keep track of time, used to switch between atlas texures
    [NonSerialized]
    private float m_Time;
    // Fo each atlas, the current texture index (used to switch between atlas texures)
    [NonSerialized]
    private int m_TextureIndex;

    // Top/left border of the first atlas preview; such border will be maintained even between atlas previews
    public const float ATLAS_PREVIEW_BORDER = 10;
    // Dimension of each atlas preview
    public const float ATLAS_PREVIEW_DIMENSION = 120;
    // Dimension of each atlas preview plus a border
    public const float ATLAS_PREVIEW_DIMENSION_PADDED = ATLAS_PREVIEW_DIMENSION + ATLAS_PREVIEW_BORDER;
    // Height of atlas labels/names
    public const float ATLAS_NAME_HEIGHT = 32;
    // colors used by atlases names
    readonly private static Color ATLAS_NAME_BACKGROUND_COLOR = new Color(1, 1, 1, 0.5f);
    readonly private static Color ATLAS_NAME_COLOR = new Color(1, 1, 1, 0.75f);
}
