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
using UnityEngine; 
using UnityEditor;

[ CustomEditor(typeof(SVGSpriteLoaderBehaviour)) ]
public class SVGSpriteLoaderEditor : Editor {

    private void OnSpriteSelect(SVGSpriteAssetFile spriteAsset)
    {
        if (this.m_EditedLoader.SpriteReference.TxtAsset != spriteAsset.SpriteRef.TxtAsset ||
            this.m_EditedLoader.SpriteReference.ElemIdx != spriteAsset.SpriteRef.ElemIdx)
        {
            // set the selected sprite (reference)
            this.m_EditedLoader.SpriteReference.TxtAsset = spriteAsset.SpriteRef.TxtAsset;
            this.m_EditedLoader.SpriteReference.ElemIdx = spriteAsset.SpriteRef.ElemIdx;
            // set the selected sprite into the renderer
            SpriteRenderer renderer = this.m_EditedLoader.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.sprite = spriteAsset.SpriteData.Sprite;
        }
    }

    private void OnAtlasSelect(SVGAtlas atlas)
    {
        if (this.m_EditedLoader.Atlas != atlas)
        {
            this.m_EditedLoader.Atlas = atlas;
            this.m_EditedLoader.SpriteReference = null;

            SpriteRenderer renderer = this.m_EditedLoader.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.sprite = null;
        }
    }

    private void DrawInspector()
    {
        bool resizeOnStart = EditorGUILayout.Toggle("Resize on Start", this.m_EditedLoader.ResizeOnStart);
        bool updateTransform = EditorGUILayout.Toggle("Update transform", this.m_EditedLoader.UpdateTransform);

        string atlasName = (this.m_EditedLoader.Atlas != null) ? this.m_EditedLoader.Atlas.name : "<select>";
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Atlas");
            if (GUILayout.Button(atlasName, "DropDown"))
                SVGAtlasSelector.Show("", this.OnAtlasSelect);
        }
        EditorGUILayout.EndHorizontal();


        if (this.m_EditedLoader.Atlas != null && this.m_EditedLoader.SpriteReference != null)
        {
            SVGSpriteAssetFile spriteAsset = this.m_EditedLoader.Atlas.GetGeneratedSprite(this.m_EditedLoader.SpriteReference);
            string buttonText = (spriteAsset != null) ? spriteAsset.SpriteData.Sprite.name : "<select>";

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Sprite");
                if (GUILayout.Button(buttonText, "DropDown"))
                    SVGSpriteSelector.Show(this.m_EditedLoader.Atlas, "", this.OnSpriteSelect);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (this.m_EditedLoader.ResizeOnStart != resizeOnStart)
        {
            this.m_EditedLoader.ResizeOnStart = resizeOnStart;
            SVGUtils.MarkObjectDirty(this.m_EditedLoader);
        }

        if (this.m_EditedLoader.UpdateTransform != updateTransform)
        {
            this.m_EditedLoader.UpdateTransform = updateTransform;
            SVGUtils.MarkObjectDirty(this.m_EditedLoader);
        }
    }

    public override void OnInspectorGUI()
    {
        // get the target object
        this.m_EditedLoader = target as SVGSpriteLoaderBehaviour;

        if (this.m_EditedLoader != null)
        {
            GUI.enabled = (Application.isPlaying) ? false : true;
            this.DrawInspector();
        }
    }

    void OnDestroy()
    {
        /*
        // avoid to leak textures
        this.DestroyCustomStyles();
        this.m_CustomStylesGenerated = false;
        */

        //this.m_EditedLoader = null;
    }

    [NonSerialized]
    private SVGSpriteLoaderBehaviour m_EditedLoader = null;
}
