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

public class SVGTextureBehaviour : MonoBehaviour
{
    private Texture2D DrawSVG(string svgXml,
                              uint texWidth, uint texHeight,
                              Color clearColor,
                              bool fastUpload)
    {
        SVGDocument document;
        SVGSurface surface;
        Texture2D texture = null;

        // create a drawing surface, with the same texture dimensions
        surface = SVGAssets.CreateSurface(texWidth, texHeight);
        if (surface == null)
        {
            return null;
        }
        // create the SVG document
        document = SVGAssets.CreateDocument(svgXml);
        if (document == null)
        {
            surface.Dispose();
            return null;
        }
        // draw the SVG document onto the surface
        if (surface.Draw(document, new SVGColor(clearColor.r, clearColor.g, clearColor.b, clearColor.a), SVGRenderingQuality.Better))
        {
            // create a 2D texture compatible with the drawing surface
            texture = surface.CreateCompatibleTexture(true, false);
            if (texture != null)
            {
                texture.hideFlags = HideFlags.DontSave;
                // copy the surface content into the texture
                if (fastUpload && Application.isPlaying)
                {
                    surface.CopyAndDestroy(texture);
                }
                else
                {
                    if (surface.Copy(texture))
                    {
                        // call Apply() so it's actually uploaded to the GPU
                        texture.Apply(false, true);
                    }
                }
            }
        }

        // destroy SVG surface and document
        surface.Dispose();
        document.Dispose();
        // return the created texture
        return texture;
    }

    // Use this for initialization
    void Start()
    {
        if (SVGFile != null)
        {
            GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
            GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
            // set texture onto our material
            GetComponent<Renderer>().material.mainTexture = DrawSVG(SVGFile.text,
                                                                    // we want at least a 1x1 texture
                                                                    (uint)Math.Max(1, TextureWidth) , (uint)Math.Max(1, TextureHeight),
                                                                    this.ClearColor, this.FastUpload);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public TextAsset SVGFile = null;
    public int TextureWidth = 512;
    public int TextureHeight = 512;
    public Color ClearColor = new Color(1, 1, 1, 1);
    public bool FastUpload = true;
}
