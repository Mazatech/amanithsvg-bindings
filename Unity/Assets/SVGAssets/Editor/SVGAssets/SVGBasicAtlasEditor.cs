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
using UnityEngine; 
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Collections.Generic;
using System.IO;

public static class ScriptableObjectUtility
{
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (path == "") 
            path = "Assets";
        else
        if (Path.GetExtension(path) != "") 
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}

// Used to keep track of dragged object inside the list of input SVG rows
public class DragInfo
{
    public DragInfo()
    {
        this.m_Dragging = false;
        this.m_Object = null;
        this.m_InsertIdx = -1;
        this.m_InsertBefore = false;
    }

    // Start a drag operation.
    public void StartDrag(System.Object obj)
    {
        this.m_Dragging = true;
        this.m_Object = obj;
        this.m_InsertIdx = -1;
        this.m_InsertBefore = false;
    }

    // Stop a drag operation.
    public void StopDrag()
    {
        this.m_Dragging = false;
        this.m_Object = null;
        this.m_InsertIdx = -1;
        this.m_InsertBefore = false;
    }

    public bool Dragging
    {
        get
        {
            return this.m_Dragging;
        }
    }

    public int InsertIdx
    {
        get
        {
            return this.m_InsertIdx;
        }

        set
        {
            this.m_InsertIdx = value;
        }
    }

    public bool InsertBefore
    {
        get
        {
            return this.m_InsertBefore;
        }

        set
        {
            this.m_InsertBefore = value;
        }
    }

    public System.Object DraggedObject
    {
        get
        {
            return this.m_Object;
        }

        set
        {
            this.m_Object = value;
        }
    }

    // True if the user is dragging a text asset or an already present SVG row, else false.
    private bool m_Dragging;
    // The dragged object.
    private System.Object m_Object;
    // Target insertion position.
    private int m_InsertIdx;
    // True/False if the dragged object must be inserted before/after the selected position.
    private bool m_InsertBefore;
}

public static class SVGBuildProcessor
{
    private static void ProcessAtlas(SVGBasicAtlas atlas)
    {
        Texture2D tmpTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        List<SVGSpriteAssetFile> spritesAssets = atlas.SpriteAssets();

        foreach (SVGSpriteAssetFile spriteAsset in spritesAssets)
        {
            SVGSpriteData spriteData = spriteAsset.SpriteData;
            Sprite original = spriteData.Sprite;

            // we must reference the original texture, because we want to keep the file reference (rd->texture.IsValid())
            Sprite tmpSprite = Sprite.Create(original.texture, new Rect(0, 0, 1, 1), new Vector2(0, 0), SVGBasicAtlas.SPRITE_PIXELS_PER_UNIT, 0, SpriteMeshType.FullRect);
            // now we change the (sprite) asset content: actually we have just reduced its rectangle to a 1x1 pixel
            EditorUtility.CopySerialized(tmpSprite, original);
        }

        for (int i = 0; i < atlas.TextureAssetsCount(); i++)
        {
            AssetFile file = atlas.TextureAsset(i);
            Texture2D original = file.Object as Texture2D;

            // copy the 1x1 texture inside the original texture
            EditorUtility.CopySerialized(tmpTexture, original);
        }
    }

    private static void ProcessScene()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        List<SVGBasicAtlas> unexportedAtlases = new List<SVGBasicAtlas>();

        // scan all game objects in the current scene, and keep track of used atlas generators
        foreach (GameObject gameObj in allObjects)
        {
            if (gameObj.activeInHierarchy)
            {
                SVGSpriteLoaderBehaviour loader = gameObj.GetComponent<SVGSpriteLoaderBehaviour>();
                if (loader != null)
                {
                    // if this atlas has not been already flagged, lets keep track of it
                    if (!loader.Atlas.Exporting)
                    {
                        unexportedAtlases.Add(loader.Atlas);
                        loader.Atlas.Exporting = true;
                    }
                }

                SVGUISpriteLoaderBehaviour uiLoader = gameObj.GetComponent<SVGUISpriteLoaderBehaviour>();
                if (uiLoader != null)
                {
                    // if this atlas has not been already flagged, lets keep track of it
                    if (!uiLoader.UIAtlas.Exporting)
                    {
                        unexportedAtlases.Add(uiLoader.UIAtlas);
                        uiLoader.UIAtlas.Exporting = true;
                    }
                }

                SVGCanvasBehaviour uiCanvas = gameObj.GetComponent<SVGCanvasBehaviour>();
                if (uiCanvas != null)
                {
                    // if this atlas has not been already flagged, lets keep track of it
                    if (!uiCanvas.UIAtlas.Exporting)
                    {
                        unexportedAtlases.Add(uiCanvas.UIAtlas);
                        uiCanvas.UIAtlas.Exporting = true;
                    }
                }
            }
        }

        foreach (SVGBasicAtlas baseAtlas in unexportedAtlases)
        {
            SVGBuildProcessor.ProcessAtlas(baseAtlas);
            // keep track of this atlas in the global list
            SVGBuildProcessor.m_Atlases.Add(baseAtlas);
        }
    }

    [PostProcessScene]
    public static void OnPostprocessScene()
    {
        if (!Application.isPlaying)
            SVGBuildProcessor.ProcessScene();
    }

    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (!Application.isPlaying)
        {
            // unflag processed atlases
            foreach (SVGAtlas atlas in SVGBuildProcessor.m_Atlases)
            {
                // update sprites using the last used scale factor
                atlas.UpdateEditorSprites(false);
                atlas.Exporting = false;
            }
            // clear the list
            SVGBuildProcessor.m_Atlases.Clear();
        }
    }

    [NonSerialized]
    private static List<SVGBasicAtlas> m_Atlases = new List<SVGBasicAtlas>();
}

public abstract class SVGBasicAtlasEditor : Editor
{
    // generate a 1x1 texture used as background for custom styles
    protected Texture2D BackgroundTextureGen(Color32 color)
    {
        Color32[] pixels = new Color32[1] { color };
        Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        // we take care to destroy the texture, when it will be the moment
        texture.hideFlags = HideFlags.DontSave;
        texture.SetPixels32(pixels);
        texture.Apply(false, true);
        return texture;
    }

    // generate custom tyles
    protected void GenerateCustomStyles()
    {
        // blue line separator
        this.m_BlueLine = new GUIStyle();
        this.m_BlueLineTexture = BackgroundTextureGen(new Color32(51, 81, 226, 255));
        this.m_BlueLine.normal.background = m_BlueLineTexture;
        // grey line separator
        this.m_GreyLine = new GUIStyle();
        this.m_GreyLineTexture = BackgroundTextureGen(new Color32(128, 128, 128, 255));
        this.m_GreyLine.normal.background = m_GreyLineTexture;
        this.m_GreyLine.padding.bottom = m_GreyLine.padding.top = 0;
        this.m_GreyLine.border.top = m_GreyLine.border.bottom = 0;
        // blue highlighted background
        this.m_HighlightRow = new GUIStyle();
        this.m_HighlightRowTexture = BackgroundTextureGen(new Color32(65, 92, 150, 255));
        this.m_HighlightRow.normal.background = m_HighlightRowTexture;
        this.m_HighlightRow.normal.textColor = Color.white;

        this.m_OffsetScaleContent = new GUIContent("Offset scale", "An additional scale factor used to adjust SVG contents globally (i.e. applied to all SVG files belonging to this atlas)");
        this.m_Pow2TexturesContent = new GUIContent("Force pow2 textures", "Force generated textures to have power-of-two dimensions");
        this.m_MaxTexturesDimensionContent = new GUIContent("Max textures dimension", "The maximum dimension (in pixels) of generated textures");
        this.m_SpritesPaddingContent = new GUIContent("Sprites padding", "Each sprite will be separated from the others by the given number of pixels (i.e. padding frame)");
        this.m_ClearColorContent = new GUIContent("Clear color", "The background color used for generated textures");
        this.m_FastUploadContent = new GUIContent("Fast upload", "Use the fast native method (OpenGL/DirectX/Metal) to upload the texture to the GPU");
        this.m_SpritesPreviewSizeContent = new GUIContent("Sprites preview size", "Dimension of sprite preview (in pixels)");
    }

    // destroy custom tyles
    protected void DestroyCustomStyles()
    {
        if (this.m_BlueLineTexture != null)
        {
            Texture2D.DestroyImmediate(this.m_BlueLineTexture);
            this.m_BlueLineTexture = null;
        }
        if (this.m_GreyLineTexture != null)
        {
            Texture2D.DestroyImmediate(this.m_GreyLineTexture);
            this.m_GreyLineTexture = null;
        }
        if (this.m_HighlightRowTexture != null)
        {
            Texture2D.DestroyImmediate(this.m_HighlightRowTexture);
            this.m_HighlightRowTexture = null;
        }
    }

    protected void OutputFolderDraw(SVGBasicAtlas atlas)
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Output folder");
            if (GUILayout.Button(atlas.OutputFolder))
            {
                string absFolderPath = EditorUtility.OpenFolderPanel("Select output folder", atlas.OutputFolder, "");
                if ((absFolderPath != "") && (absFolderPath.StartsWith(Application.dataPath)))
                {
                    string relFolderPath = "Assets" + absFolderPath.Substring(Application.dataPath.Length);
                    if (AssetDatabase.IsValidFolder(relFolderPath))
                    {
                        atlas.OutputFolder = relFolderPath;
                    }
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        // textures output subfolder
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Textures subfolder");
            EditorGUILayout.LabelField(atlas.TexturesSubFolder);
            //EditorGUILayout.SelectableLabel(atlas.TexturesFolder(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        }
        EditorGUILayout.EndHorizontal();
        // sprites output subfolder
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Sprites subfolder");
            EditorGUILayout.LabelField(atlas.SpritesSubFolder);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(18);
        EditorGUILayout.LabelField("Drag & drop SVG assets here");
    }

    protected bool UpdateButtonDraw(SVGBasicAtlas atlas)
    {
        bool pushed;

        // update button
        string updateStr = (atlas.NeedsUpdate()) ? "Update *" : "Update";
        if (GUILayout.Button(updateStr))
        {
            // close all modal popup editors
            SVGPivotEditor.CloseAll();
            SVGSpriteSelector.CloseAll();
            SVGAtlasSelector.CloseAll();
            pushed = true;
        }
        else
        {
            pushed = false;
        }

        return pushed;
    }

    protected abstract bool SvgInputAssetDrawImplementation(SVGBasicAtlas atlas, SVGAssetInput svgAsset, int svgAssetIndex);

    private bool SvgInputAssetDraw(SVGBasicAtlas atlas, int index, out Rect rowRect)
    {
        bool isDirty;
        SVGAssetInput svgAsset = atlas.SvgAsset(index);
        bool highlight = (this.m_DragInfo.Dragging && this.m_DragInfo.DraggedObject == svgAsset) ? true : false;

        if (this.m_DragInfo.InsertIdx == index && this.m_DragInfo.InsertBefore)
        {
            // draw a separator before the row
            GUILayout.Box(GUIContent.none, this.m_BlueLine, GUILayout.ExpandWidth(true), GUILayout.Height(2));
        }

        // if the SVG row is the dragged one, change colors
        if (highlight)
        {
            EditorGUILayout.BeginHorizontal(this.m_HighlightRow);
            // a row: asset name, separate groups checkbox, remove button, instantiate button
            EditorGUILayout.LabelField(svgAsset.TxtAsset.name, this.m_HighlightRow, GUILayout.MinWidth(10));
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            // a row: asset name, separate groups checkbox, remove button, instantiate button
            EditorGUILayout.LabelField(svgAsset.TxtAsset.name, GUILayout.MinWidth(10));
        }

        isDirty = this.SvgInputAssetDrawImplementation(atlas, svgAsset, index);

        EditorGUILayout.EndHorizontal();
        rowRect = GUILayoutUtility.GetLastRect();

        if (this.m_DragInfo.InsertIdx == index && (!this.m_DragInfo.InsertBefore))
        {
            // draw a separator after the row
            GUILayout.Box(GUIContent.none, this.m_BlueLine, GUILayout.ExpandWidth(true), GUILayout.Height(2));
        }

        return isDirty;
    }

    protected bool SvgInputAssetsDraw(SVGBasicAtlas atlas, Event currentEvent, out Rect scollRect)
    {
        bool isDirty = false;

        // keep track of drawn rows
        if (currentEvent.type != EventType.Layout)
        {
            this.m_InputAssetsRects = new List<Rect>();
        }

        Vector2 scrollPos = EditorGUILayout.BeginScrollView(this.m_SvgListScrollPos, GUILayout.ExpandWidth(true), GUILayout.MaxHeight(102), GUILayout.Height(102));
        // perform backward loop because we could even remove entries without issues
        for (int i = 0; i < atlas.SvgAssetsCount(); ++i)
        {
            Rect rowRect;
            isDirty |= this.SvgInputAssetDraw(atlas, i, out rowRect);
            // keep track of row rectangle
            if (currentEvent.type != EventType.Layout)
            {
                this.m_InputAssetsRects.Add(rowRect);
            }
        }
        EditorGUILayout.EndScrollView();
        // keep track of the scrollview area
        scollRect = GUILayoutUtility.GetLastRect();
        if (this.m_SvgListScrollPos != scrollPos)
        {
            this.m_SvgListScrollPos = scrollPos;
        }
        return isDirty;
    }

    protected bool HandleDragEvents(SVGBasicAtlas atlas, Event currentEvent, Rect scollRect)
    {
        int i;
        bool isDirty = false;

        // events handler
        if (currentEvent.type != EventType.Layout)
        {
            bool needRepaint = false;
            // get mouse position relative to scollRect
            Vector2 mousePos = currentEvent.mousePosition - new Vector2(scollRect.xMin, scollRect.yMin);

            if (scollRect.Contains(currentEvent.mousePosition))
            {
                bool separatorInserted = false;

                for (i = 0; i < atlas.SvgAssetsCount(); ++i)
                {
                    // get the row rectangle relative to atlas.SvgList[i]
                    Rect rowRect = this.m_InputAssetsRects[i];
                    // expand the rectangle height
                    rowRect.yMin -= 3;
                    rowRect.yMax += 3;

                    if (rowRect.Contains(mousePos))
                    {
                        // a mousedown on a row, will stop an already started drag operation
                        if (currentEvent.type == EventType.MouseDown)
                        {
                            this.m_DragInfo.StopDrag();
                        }
                        // check if we are already dragging an object
                        if (this.m_DragInfo.Dragging)
                        {
                            if (!separatorInserted)
                            {
                                bool ok = true;
                                bool dragBefore = (mousePos.y <= rowRect.yMin + rowRect.height / 2) ? true : false;
                                // if we are dragging a text (asset) file, all positions are ok
                                // if we are dragging an already present SVG row, we must perform additional checks
                                if (!(this.m_DragInfo.DraggedObject is TextAsset))
                                {
                                    if (this.m_DragInfo.DraggedObject == atlas.SvgAsset(i))
                                    {
                                        ok = false;
                                    }
                                    else
                                    {
                                        if (dragBefore)
                                        {
                                            if (i > 0 && this.m_DragInfo.DraggedObject == atlas.SvgAsset(i - 1))
                                            {
                                                ok = false;
                                            }
                                        }
                                        else
                                        {
                                            if (i < (atlas.SvgAssetsCount() - 1) && this.m_DragInfo.DraggedObject == atlas.SvgAsset(i + 1))
                                            {
                                                ok = false;
                                            }
                                        }
                                    }
                                }

                                if (ok)
                                {
                                    if (dragBefore)
                                    {
                                        this.m_DragInfo.InsertIdx = i;
                                        this.m_DragInfo.InsertBefore = true;
                                        separatorInserted = true;
                                    }
                                    else
                                    {
                                        this.m_DragInfo.InsertIdx = i;
                                        this.m_DragInfo.InsertBefore = false;
                                        separatorInserted = true;
                                    }
                                    needRepaint = true;
                                }
                            }
                        }
                        else
                        {
                            // initialize the drag of an already present SVG document
                            if (currentEvent.type == EventType.MouseDrag)
                            {
                                DragAndDrop.PrepareStartDrag();
                                DragAndDrop.StartDrag("Start drag");
                                this.m_DragInfo.StartDrag(atlas.SvgAsset(i));
                                needRepaint = true;
                            }
                        }
                    }
                }

                // mouse is dragging inside the drop box, but not under an already present row; insertion point is inside the last element
                if (this.m_DragInfo.Dragging && !separatorInserted && atlas.SvgAssetsCount() > 0 && mousePos.y > this.m_InputAssetsRects[atlas.SvgAssetsCount() - 1].yMax)
                {
                    bool ok = true;

                    if (!(this.m_DragInfo.DraggedObject is TextAsset))
                    {
                        if (this.m_DragInfo.DraggedObject == atlas.SvgAsset(atlas.SvgAssetsCount() - 1))
                            ok = false;
                    }

                    if (ok)
                    {
                        this.m_DragInfo.InsertIdx = atlas.SvgAssetsCount() - 1;
                        this.m_DragInfo.InsertBefore = false;
                        needRepaint = true;
                    }
                }
            }
            else
            {
                this.m_DragInfo.InsertIdx = -1;
            }

            if (needRepaint)
            {
                Repaint();
            }
        }

        //if (currentEvent.type == EventType.MouseUp || currentEvent.rawType == EventType.MouseUp || currentEvent.type == EventType.DragExited)
        if (currentEvent.type == EventType.DragExited)
        {
            this.m_DragInfo.StopDrag();
            DragAndDrop.objectReferences = new UnityEngine.Object[0];
        }
        else
        {
            switch (currentEvent.type) {

            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (this.m_DragInfo.Dragging)
                {
                    bool dragValid = true;

                    if (scollRect.Contains(currentEvent.mousePosition) && dragValid)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        if (currentEvent.type == EventType.DragPerform)
                        {
                            int index;

                            // accept drag&drop operation
                            DragAndDrop.AcceptDrag();
                            // check if we are dropping a text asset
                            if (this.m_DragInfo.DraggedObject is TextAsset)
                            {
                                // if a valid inter-position has not been selected, append the new asset at the end of list
                                if (this.m_DragInfo.InsertIdx < 0)
                                {
                                    index = atlas.SvgAssetsCount();
                                }
                                else
                                {
                                    index = (this.m_DragInfo.InsertBefore) ? this.m_DragInfo.InsertIdx : (this.m_DragInfo.InsertIdx + 1);
                                }
                                // add the text asset to the SVG list
                                if (atlas.SvgAssetAdd(this.m_DragInfo.DraggedObject as TextAsset, index))
                                {
                                    isDirty = true;
                                }
                            }
                            else
                            {
                                // we are dropping an already present SVG row
                                index = (this.m_DragInfo.InsertBefore) ? this.m_DragInfo.InsertIdx : (this.m_DragInfo.InsertIdx + 1);
                                if (atlas.SvgAssetMove(this.m_DragInfo.DraggedObject as SVGAssetInput, index))
                                {
                                    isDirty = true;
                                }
                            }
                            // now we can close the drag operation
                            this.m_DragInfo.StopDrag();
                        }
                    }
                    else
                    {
                        // if we are dragging outside of the allowed drop region, simply reject the drag&drop
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }
                }
                else
                {
                    if (scollRect.Contains(currentEvent.mousePosition))
                    {
                        if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
                        {
                            UnityEngine.Object draggedObject = DragAndDrop.objectReferences[0];
                            // check object type, only TextAssets are allowed
                            if (draggedObject is TextAsset)
                            {
                                this.m_DragInfo.StartDrag(DragAndDrop.objectReferences[0]);
                                Repaint();
                            }
                            else
                            {
                                // acceptance is not confirmed (e.g. we are dragging a binary file)
                                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                            }
                        }
                    }
                }
                break;

            default:
                break;
            }
        }

        return isDirty;
    }

    public override void OnInspectorGUI()
    {
        if (!this.m_CustomStylesGenerated)
        {
            this.GenerateCustomStyles();
            this.m_CustomStylesGenerated = true;
        }
        GUI.enabled = (Application.isPlaying) ? false : true;
    }

    void OnDestroy()
    {
        // avoid to leak textures
        this.DestroyCustomStyles();
        this.m_CustomStylesGenerated = false;
    }

    // Custom styles
    [NonSerialized]
    protected bool m_CustomStylesGenerated = false;
    [NonSerialized]
    protected Texture2D m_BlueLineTexture = null;
    [NonSerialized]
    protected GUIStyle m_BlueLine = null;
    [NonSerialized]
    protected Texture2D m_GreyLineTexture = null;
    [NonSerialized]
    protected GUIStyle m_GreyLine = null;
    [NonSerialized]
    protected Texture2D m_HighlightRowTexture = null;
    [NonSerialized]
    protected GUIStyle m_HighlightRow = null;

    [NonSerialized]
    protected GUIContent m_OffsetScaleContent = null;
    [NonSerialized]
    protected GUIContent m_Pow2TexturesContent = null;
    [NonSerialized]
    protected GUIContent m_MaxTexturesDimensionContent = null;
    [NonSerialized]
    protected GUIContent m_SpritesPaddingContent = null;
    [NonSerialized]
    protected GUIContent m_ClearColorContent = null;
    [NonSerialized]
    protected GUIContent m_FastUploadContent = null;
    [NonSerialized]
    protected GUIContent m_SpritesPreviewSizeContent = null;

    protected List<Rect> m_InputAssetsRects = null;
    protected DragInfo m_DragInfo = new DragInfo();
    // Current scroll position inside the list of input SVG
    protected Vector2 m_SvgListScrollPos = new Vector2(0, 0);
    // Current scroll position inside the list of generated sprites
    protected Vector2 m_SvgSpritesScrollPos = new Vector2(0, 0);

    static public string LastOutputFolder = "Assets";
}
