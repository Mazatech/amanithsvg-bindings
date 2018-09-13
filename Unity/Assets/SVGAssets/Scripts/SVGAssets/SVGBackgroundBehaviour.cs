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
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public enum SVGBackgroundScaleType {
    // Scale SVG background according to desired width.
    Horizontal          = 0,
    // Scale SVG background according to desired height.
    Vertical            = 1
};

[ExecuteInEditMode]
public class SVGBackgroundBehaviour : MonoBehaviour
{
    private void LoadSVG()
    {
        // create and load SVG document
        this.m_SvgDoc = (this.SVGFile != null) ? SVGAssets.CreateDocument(this.SVGFile.text) : null;
    }

    public void DestroyAll(bool fullDestroy)
    {
        // set an empty sprite
        SpriteRenderer renderer = this.gameObject.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.sprite = null;
        // destroy SVG document, if loaded
        if (this.m_SvgDoc != null && fullDestroy)
        {
            this.m_SvgDoc.Dispose();
            this.m_SvgDoc = null;
        }
        // destroy sprite
        if (this.m_Sprite != null)
        {
        #if UNITY_EDITOR
            DestroyImmediate(this.m_Sprite);
        #else
            Destroy(this.m_Sprite);
        #endif
            this.m_Sprite = null;
        }
        // destroy texture
        if (this.m_Texture != null)
        {
        #if UNITY_EDITOR
            DestroyImmediate(this.m_Texture);
        #else
            Destroy(this.m_Texture);
        #endif
            this.m_Texture = null;
        }
    }
    
    private Texture2D DrawSVG(uint texWidth, uint texHeight, SVGColor clearColor, bool fastUpload)
    {
        SVGSurface surface;
        Texture2D texture = null;

        // create a drawing surface, with the same texture dimensions
        surface = SVGAssets.CreateSurface(texWidth, texHeight);
        if (surface == null)
            return null;
        // draw the SVG document onto the surface
        if (surface.Draw(this.m_SvgDoc, clearColor, SVGRenderingQuality.Better))
        {
            // create a 2D texture compatible with the drawing surface
            texture = surface.CreateCompatibleTexture(true, false);
            if (texture != null)
            {
                texture.hideFlags = HideFlags.HideAndDontSave;
                // copy the surface content into the texture
                if (fastUpload && Application.isPlaying)
                    surface.CopyAndDestroy(texture);
                else
                {
                    if (surface.Copy(texture))
                        // call Apply() so it's actually uploaded to the GPU
                        texture.Apply(false, true);
                }
            }
        }
        // destroy SVG surface and document
        surface.Dispose();
        // return the created texture
        return texture;
    }

    private Pair<Texture2D, Sprite> UpdateTexture(float desiredWidth, float desiredHeight, out float downScale)
    {
        float maxAllowedDimension = (float)SVGSurface.MaxDimension;
        float texWidth = desiredWidth;
        float texHeight = desiredHeight;
        float maxTexDimension = Math.Max(texWidth, texHeight);
        // we cannot exceed the maximum allowed surface dimension
        if (maxTexDimension > maxAllowedDimension)
        {
            float scl = maxAllowedDimension / maxTexDimension;
            texWidth *= scl;
            texHeight *= scl;
            downScale = scl;
        }
        else
            downScale = 1;
        // we want at least a 1x1 texture
        texWidth = Math.Max(Mathf.Floor(texWidth), 1);
        texHeight = Math.Max(Mathf.Floor(texHeight), 1);
        Texture2D texture = this.DrawSVG((uint)texWidth, (uint)texHeight, new SVGColor(this.ClearColor.r, this.ClearColor.g, this.ClearColor.b, this.ClearColor.a), this.FastUpload);
        if (texture != null)
        {
            // create the sprite equal to the whole texture (pivot in the middle)
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), SVGBasicAtlas.SPRITE_PIXELS_PER_UNIT, 0, SpriteMeshType.FullRect);
            sprite.hideFlags = HideFlags.HideAndDontSave;
            return(new Pair<Texture2D, Sprite>(texture, sprite));
        }
        else
            return(new Pair<Texture2D, Sprite>(null, null));
    }

    private Pair<Texture2D, Sprite> UpdateTexture(float desiredSize, out float downScale)
    {
        float texWidth, texHeight;
        
        if (this.ScaleAdaption == SVGBackgroundScaleType.Horizontal)
        {
            texWidth = desiredSize;
            texHeight = (this.m_SvgDoc.Viewport.Height / this.m_SvgDoc.Viewport.Width) * texWidth;
        }
        else
        {
            texHeight = desiredSize;
            texWidth = (this.m_SvgDoc.Viewport.Width / this.m_SvgDoc.Viewport.Height) * texHeight;
        }
        
        return this.UpdateTexture(texWidth, texHeight, out downScale);
    }

    private void UpdateBackground()
    {
        if (this.m_SvgDoc != null)
        {
            SpriteRenderer renderer = this.gameObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                if ((this.Sliced && this.Size >= 1) || ((!this.Sliced) && this.SlicedWidth >= 1 && this.SlicedHeight >= 1))
                {
                    float downScale;
                    Pair<Texture2D, Sprite> texSpr;

                    if (this.Sliced)
                    {
                        this.m_SvgDoc.AspectRatio.MeetOrSlice = SVGMeetOrSlice.Slice;
                        texSpr = this.UpdateTexture(this.SlicedWidth, this.SlicedHeight, out downScale);
                    }
                    else
                    {
                        this.m_SvgDoc.AspectRatio.MeetOrSlice = SVGMeetOrSlice.Meet;
                        texSpr = this.UpdateTexture(this.Size, out downScale);
                    }

                    if (texSpr.First != null && texSpr.Second != null)
                    {
                        this.m_Texture = texSpr.First;
                        this.m_Sprite = texSpr.Second;
                        renderer.sprite = this.m_Sprite;
                        this.transform.localScale = new Vector3(downScale, downScale, 1);
                    }
                }
            }
        }
    }

    // For a non-sliced background, calculate the scale adaption and size parameters such that the background will cover the specified screen dimensions
    public Pair<SVGBackgroundScaleType, int> CoverFullScreen(int screenWidth, int screenHeight)
    {
        // load background SVG, if needed
        if (this.m_SvgDoc == null)
            this.LoadSVG();

        if (this.m_SvgDoc != null)
        {
            // try horizontal
            float texWidth = (float)screenWidth;
            float texHeight = (this.m_SvgDoc.Viewport.Height / this.m_SvgDoc.Viewport.Width) * texWidth;

            if (texHeight >= (float)screenHeight)
                return new Pair<SVGBackgroundScaleType, int>(SVGBackgroundScaleType.Horizontal, screenWidth);
            else
                return new Pair<SVGBackgroundScaleType, int>(SVGBackgroundScaleType.Vertical, screenHeight);
        }
        return new Pair<SVGBackgroundScaleType, int>(SVGBackgroundScaleType.Horizontal, 0);
    }

    public void UpdateBackground(bool fullUpdate)
    {
        // destroy current texture and sprite (if specified, destroy the SVG document too)
        this.DestroyAll(fullUpdate);

        if (this.SVGFile != null)
        {
            // load background SVG, if needed
            if (this.m_SvgDoc == null)
                this.LoadSVG();
            // update the background with the desired size
            this.UpdateBackground();
        }
    }

    public float PixelWidth
    {
        // get the background sprite width, in pixels
        get
        {
            return ((this.m_Texture != null) ? (float)this.m_Texture.width : 0);
        }
    }

    public float PixelHeight
    {
        // get the background sprite height, in pixels
        get
        {
            return ((this.m_Texture != null) ? (float)this.m_Texture.height : 0);
        }
    }

    public float WorldWidth
    {
        // get the background sprite width, in world coordinates
        get
        {
            return (this.PixelWidth / SVGBasicAtlas.SPRITE_PIXELS_PER_UNIT);
        }
    }

    public float WorldHeight
    {
        // get the background sprite height, in world coordinates
        get
        {
            return (this.PixelHeight / SVGBasicAtlas.SPRITE_PIXELS_PER_UNIT);
        }
    }

    public float WorldLeft
    {
        // get the background sprite x-coordinate relative to its left edge, in world coordinates
        get
        {
            return this.gameObject.transform.position.x - (this.WorldWidth / 2);
        }
    }

    public float WorldRight
    {
        // get the background sprite x-coordinate relative to its right edge, in world coordinates
        get
        {
            return this.gameObject.transform.position.x + (this.WorldWidth / 2);
        }
    }

    void Start()
    {
        if (this.GenerateOnStart)
            this.UpdateBackground(true);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnEnable()
    {
    #if UNITY_EDITOR
        if (!Application.isPlaying)
            this.UpdateBackground(true);
    #endif
    }

#if UNITY_EDITOR
    private bool RequirementsCheck()
    {
        SpriteRenderer renderer = this.gameObject.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            EditorUtility.DisplayDialog("Incompatible game object",
                                        string.Format("In order to work properly, the component {0} must be attached to a sprite object", this.GetType()),
                                        "Ok");
            DestroyImmediate(this);
            return false;
        }
        return true;
    }

    void Reset()
    {
        if (this.RequirementsCheck())
        {
            this.SVGFile = null;
            this.ScaleAdaption = SVGBackgroundScaleType.Horizontal;
            this.Size = 256;
            this.ClearColor = new Color(1, 1, 1, 0);
            this.GenerateOnStart = true;
            this.FastUpload = true;

            this.Sliced = false;
            this.SlicedWidth = 256;
            this.SlicedHeight = 256;

            this.m_SvgDoc = null;
            this.m_Texture = null;
            this.m_Sprite = null;
            this.DestroyAll(true);
        }
    }
#endif

    // The background SVG text asset
    public TextAsset SVGFile;
    // Scale adaption
    public SVGBackgroundScaleType ScaleAdaption;
    // The size (horizontal or vertical), in pixels
    public int Size;
    // Clear color
    public Color ClearColor;
    public bool GenerateOnStart;
    public bool FastUpload;

    public bool Sliced;
    public int SlicedWidth;
    public int SlicedHeight;

    // The loaded SVG document
    [NonSerialized]
    private SVGDocument m_SvgDoc;
    // Background sprite texture
    [NonSerialized]
    private Texture2D m_Texture;
    // Background sprite
    [NonSerialized]
    private Sprite m_Sprite;
}
