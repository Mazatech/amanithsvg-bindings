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
#if UNITY_2_6
    #define UNITY_2_X
    #define UNITY_2_PLUS
#elif UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
    #define UNITY_3_X
    #define UNITY_2_PLUS
    #define UNITY_3_PLUS
#elif UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
    #define UNITY_4_X
    #define UNITY_2_PLUS
    #define UNITY_3_PLUS
    #define UNITY_4_PLUS
#elif UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_4_OR_NEWER
    #define UNITY_5_X
    #define UNITY_2_PLUS
    #define UNITY_3_PLUS
    #define UNITY_4_PLUS
    #define UNITY_5_PLUS
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;
    using UnityEditor;
    using UnityEditorInternal;
    using System.Reflection;
#endif

public class SVGAtlas : SVGBasicAtlas
{
#if UNITY_EDITOR

    private void FixAnimationClip(AnimationClip clip, float deltaScaleX, float deltaScaleY)
    {
     #if UNITY_5_4_OR_NEWER
        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(clip);

        foreach (var binding in curveBindings)
        {
            // check for keyframe animation of localPosition
            bool localPosX = (binding.propertyName.IndexOf("LocalPosition.x", StringComparison.OrdinalIgnoreCase) >= 0) ? true : false;
            bool localPosY = (binding.propertyName.IndexOf("LocalPosition.y", StringComparison.OrdinalIgnoreCase) >= 0) ? true : false;
            if (localPosX || localPosY)
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                // get the scale factor
                float deltaScale = localPosX ? deltaScaleX : deltaScaleY;

                // "Note that the keys array is by value, i.e. getting keys returns a copy of all keys and setting keys copies them into the curve"
                Keyframe[] keys = curve.keys;
                for (int i = 0; i < keys.Length; ++i) {
                    keys[i].value *= deltaScale;
                    keys[i].inTangent *= deltaScale;
                    keys[i].outTangent *= deltaScale;
                }
                curve.keys = keys;
                // set the new keys
                clip.SetCurve(binding.path, typeof(Transform)/*curveData.type*/, binding.propertyName, curve);
            }
        }
    #else
        AnimationClipCurveData[] curves = AnimationUtility.GetAllCurves(clip, true);
        foreach (AnimationClipCurveData curveData in curves)
        {
            // check for keyframe animation of localPosition
            bool localPosX = (curveData.propertyName.IndexOf("LocalPosition.x", StringComparison.OrdinalIgnoreCase) >= 0) ? true : false;
            bool localPosY = (curveData.propertyName.IndexOf("LocalPosition.y", StringComparison.OrdinalIgnoreCase) >= 0) ? true : false;

            if (localPosX || localPosY)
            {
                AnimationCurve curve = curveData.curve;
                // get the scale factor
                float deltaScale = localPosX ? deltaScaleX : deltaScaleY;

                // "Note that the keys array is by value, i.e. getting keys returns a copy of all keys and setting keys copies them into the curve"
                Keyframe[] keys = curve.keys;
                for (int i = 0; i < keys.Length; ++i) {
                    keys[i].value *= deltaScale;
                    keys[i].inTangent *= deltaScale;
                    keys[i].outTangent *= deltaScale;
                }
                curve.keys = keys;
                // set the new keys
                clip.SetCurve(curveData.path, curveData.type, curveData.propertyName, curve);
            }
        }
    #endif
    }

    private void FixPositions(GameObject gameObj, float deltaScaleX, float deltaScaleY)
    {
        Vector3 newPos = gameObj.transform.localPosition;
        newPos.x *= deltaScaleX;
        newPos.y *= deltaScaleY;
        gameObj.transform.localPosition = newPos;

        // fix Animation components
        Animation[] animations = gameObj.GetComponents<Animation>();
        foreach (Animation animation in animations)
        {
            foreach (AnimationState animState in animation)
            {
                if (animState.clip != null)
                    this.FixAnimationClip(animState.clip, deltaScaleX, deltaScaleY);
            }
        }

        // fix Animator components
        Animator[] animators = gameObj.GetComponents<Animator>();
        foreach (Animator animator in animators)
        {
            UnityEditor.Animations.AnimatorController animController = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
        #if UNITY_5_PLUS
            for (int i = 0; i < animController.layers.Length; i++)
        #else
            for (int i = 0; i < animator.layerCount; i++)
        #endif
            {
            #if UNITY_5_PLUS
                UnityEditor.Animations.AnimatorStateMachine stateMachine = animController.layers[i].stateMachine;
                for (int j = 0; j < stateMachine.states.Length; j++)
            #else
                UnityEditor.Animations.AnimatorStateMachine stateMachine = animController.GetLayer(i).stateMachine;
                for (int j = 0; j < stateMachine.stateCount; j++)
            #endif
                {
                #if UNITY_5_PLUS
                    UnityEditor.Animations.ChildAnimatorState state = stateMachine.states[j];
                    Motion mtn = state.state.motion;
                #else
                    UnityEditor.Animations.AnimatorState state = stateMachine.GetState(j);
                    Motion mtn = state.GetMotion();
                #endif

                    if (mtn != null)
                    {
                        AnimationClip clip = mtn as AnimationClip;
                        this.FixAnimationClip(clip, deltaScaleX, deltaScaleY);
                    }
                }
            }
        }
    }

    private void GetSpritesInstances(List<GameObject> spritesInstances)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        
        foreach (GameObject gameObj in allObjects)
        {
            // check if the game object is an "SVG sprite" instance of this atlas generator
            if (gameObj.activeInHierarchy)
            {
                SVGSpriteLoaderBehaviour loader = gameObj.GetComponent<SVGSpriteLoaderBehaviour>();
                // we must be sure that the loader component must refer to this atlas
                if (loader != null && loader.Atlas == this)
                    // add this instance to the output lists
                    spritesInstances.Add(gameObj);
            }
        }
    }

    private void UpdateEditorSprites(float newScale)
    {
        // get the list of instantiated SVG sprites
        List<GameObject> spritesInstances = new List<GameObject>();
        this.GetSpritesInstances(spritesInstances);
        // regenerate the list of sprite locations
        this.m_GeneratedSpritesLists = new SVGSpritesListDictionary();

        if (this.m_SvgList.Count <= 0)
        {
            AssetDatabase.StartAssetEditing();
            // delete previously generated textures (i.e. get all this.GeneratedTextures entries and delete the relative files)
            this.DeleteTextures();
            // delete previously generated sprites (i.e. get all this.GeneratedSprites entries and delete the relative files)
            this.DeleteSprites();

            if (spritesInstances.Count > 0)
            {
                bool remove = EditorUtility.DisplayDialog("Missing sprite!",
                                                          string.Format("{0} gameobjects reference sprites that do not exist anymore. Would you like to remove them from the scene?", spritesInstances.Count),
                                                          "Remove", "Keep");
                if (remove)
                    this.DeleteGameObjects(spritesInstances);
            }
            AssetDatabase.StopAssetEditing();
            // input SVG list is empty, simply reset both hash
            this.m_SvgListHashOld = this.m_SvgListHashCurrent = "";
            return;
        }

        // generate textures and sprites
        List<Texture2D> textures = new List<Texture2D>();
        List<KeyValuePair<SVGSpriteRef, SVGSpriteData>> sprites = new List<KeyValuePair<SVGSpriteRef, SVGSpriteData>>();

        if (SVGRuntimeGenerator.GenerateSprites(// input
                                                this.m_SvgList, this.m_MaxTexturesDimension, this.m_SpritesBorder, this.m_Pow2Textures, newScale, this.m_ClearColor, this.m_FastUpload, this.m_GeneratedSpritesFiles,
                                                // output
                                                textures, sprites, this.m_GeneratedSpritesLists))
        {
            int i, j;

            if ((this.m_EditorGenerationScale > 0) && (newScale != this.m_EditorGenerationScale))
            {
                // calculate how much we have to scale (relative) positions
                float deltaScale = newScale / this.m_EditorGenerationScale;
                // fix objects positions and animations
                foreach (GameObject gameObj in spritesInstances)
                {
                    this.FixPositions(gameObj, deltaScale, deltaScale);
                }
            }
            // keep track of the new generation scale
            this.m_EditorGenerationScale = newScale;

            AssetDatabase.StartAssetEditing();
            // delete previously generated textures (i.e. get all this.GeneratedTextures entries and delete the relative files)
            this.DeleteTextures();
            // delete previously generated sprites (i.e. get all this.GeneratedSprites entries and delete the relative files)
            this.DeleteSprites();
            // ensure the presence of needed subdirectories
            string atlasesPath = this.CreateOutputFolders();
            string texturesDir = atlasesPath + "/Textures/";
            string spritesDir = atlasesPath + "/Sprites/";
            // save new texture assets
            i = 0;
            foreach (Texture2D texture in textures)
            {
                string textureFileName = texturesDir + "texture" + i + ".asset";
                // save texture
                AssetDatabase.CreateAsset(texture, textureFileName);

                // DEBUG STUFF
                //byte[] pngData = texture.EncodeToPNG();
                //if (pngData != null)
                //  System.IO.File.WriteAllBytes(texturesDir + "texture" + i + ".png", pngData);

                // keep track of the saved texture
                this.m_GeneratedTexturesFiles.Add(new AssetFile(textureFileName, texture));
                i++;
            }
            // save sprite assets
            j = sprites.Count;
            for (i = 0; i < j; ++i)
            {
                // get sprite reference and its pivot
                SVGSpriteRef spriteRef = sprites[i].Key;
                SVGSpriteData spriteData = sprites[i].Value;

                // build sprite file name
                string spriteFileName = spritesDir + spriteData.Sprite.name + ".asset";
                // save sprite asset
                AssetDatabase.CreateAsset(spriteData.Sprite, spriteFileName);
                // keep track of the saved sprite and its pivot
                this.m_GeneratedSpritesFiles.Add(spriteRef, new SVGSpriteAssetFile(spriteFileName, spriteRef, spriteData));
            }
            AssetDatabase.StopAssetEditing();

            // for already instantiated (SVG) game object, set the new sprites
            // in the same loop we keep track of those game objects that reference missing sprites (i.e. sprites that do not exist anymore)
            List<GameObject> missingSpriteObjs = new List<GameObject>();
            foreach (GameObject gameObj in spritesInstances)
            {
                SVGSpriteAssetFile spriteAsset;
                SVGSpriteLoaderBehaviour spriteLoader = (SVGSpriteLoaderBehaviour)gameObj.GetComponent<SVGSpriteLoaderBehaviour>();
                
                if (spriteLoader.SpriteReference.TxtAsset != null)
                {
                    if (this.m_GeneratedSpritesFiles.TryGetValue(spriteLoader.SpriteReference, out spriteAsset))
                    {
                        // link the new sprite to the renderer
                        SpriteRenderer renderer = (SpriteRenderer)gameObj.GetComponent<SpriteRenderer>();
                        if (renderer != null)
                        {
                            SVGSpriteData spriteData = spriteAsset.SpriteData;
                            // assign the new sprite
                            renderer.sprite = spriteData.Sprite;
                            // NB: existing instances do not change sorting order!
                        }
                    }
                    else
                        missingSpriteObjs.Add(gameObj);
                }
            }

            if (missingSpriteObjs.Count > 0)
            {
                bool remove = EditorUtility.DisplayDialog("Missing sprite!",
                                                          string.Format("{0} gameobjects reference sprites that do not exist anymore. Would you like to remove them from the scene?", missingSpriteObjs.Count),
                                                          "Remove", "Keep");
                if (remove)
                    this.DeleteGameObjects(missingSpriteObjs);
            }

            // now SVG documents are instantiable
            foreach (SVGAssetInput svgAsset in this.m_SvgList)
                svgAsset.Instantiable = true;
            // keep track of the new hash
            this.m_SvgListHashOld = this.m_SvgListHashCurrent;
        }
    }

    // used by SVGAtlasEditor, upon the "Update" button click (recalcScale = true); this function is used even
    // in the build post-processor, in order to restore sprites (recalcScale = false)
    public void UpdateEditorSprites(bool recalcScale)
    {
        float newScale;

        if (recalcScale)
        {
            Vector2 gameViewRes = SVGUtils.GetGameView();

            float currentWidth = (this.m_DeviceTestWidth <= 0) ? gameViewRes.x : (float)this.m_DeviceTestWidth;
            float currentHeight = (this.m_DeviceTestHeight <= 0) ? gameViewRes.y : (float)this.m_DeviceTestHeight;
            newScale = SVGRuntimeGenerator.ScaleFactorCalc((float)this.m_ReferenceWidth, (float)this.m_ReferenceHeight, currentWidth, currentHeight, this.m_ScaleType, this.m_OffsetScale);
        }
        else
            newScale = this.m_EditorGenerationScale;

        this.UpdateEditorSprites(newScale);
    }

    private static void UpdatePivotHierarchy(GameObject gameObj, Vector2 delta, uint depthLevel)
    {
        SVGSpriteLoaderBehaviour loader = gameObj.GetComponent<SVGSpriteLoaderBehaviour>();

        if (loader != null)
        {
            Vector2 realDelta = (depthLevel > 0) ? (new Vector2(-delta.x, -delta.y)) : (new Vector2(delta.x * gameObj.transform.localScale.x, delta.y * gameObj.transform.localScale.y));
            Vector2 newPos = new Vector2(gameObj.transform.localPosition.x + realDelta.x, gameObj.transform.localPosition.y + realDelta.y);
            // modify the current node
            gameObj.transform.localPosition = newPos;
        }

        // traverse children
        int j = gameObj.transform.childCount;
        for (int i = 0; i < j; ++i)
        {
            GameObject child = gameObj.transform.GetChild(i).gameObject;
            SVGAtlas.UpdatePivotHierarchy(child, delta, depthLevel + 1);
        }
    }

    public void UpdatePivot(SVGSpriteAssetFile spriteAsset, Vector2 newPivot)
    {
        SVGSpriteRef spriteRef = spriteAsset.SpriteRef;
        SVGSpriteData spriteData = spriteAsset.SpriteData;
        Sprite oldSprite = spriteData.Sprite;
        // keep track of pivot movement
        Vector2 deltaPivot = newPivot - spriteData.Pivot;
        Vector2 deltaMovement = (new Vector2(deltaPivot.x * oldSprite.rect.width, deltaPivot.y * oldSprite.rect.height)) / SVGBasicAtlas.SPRITE_PIXELS_PER_UNIT;
        // create a new sprite (same texture, same rectangle, different pivot)
        Sprite newSprite = Sprite.Create(oldSprite.texture, oldSprite.rect, newPivot, SVGBasicAtlas.SPRITE_PIXELS_PER_UNIT, 0, SpriteMeshType.FullRect, spriteData.Border);
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject gameObj in allObjects)
        {
            if (gameObj.activeInHierarchy)
            {
                SVGSpriteLoaderBehaviour loader = gameObj.GetComponent<SVGSpriteLoaderBehaviour>();
                // we must be sure that the loader component must refer to this atlas
                if (loader != null && loader.Atlas == this)
                {
                    // check if the instance uses the specified sprite
                    if (loader.SpriteReference.TxtAsset == spriteRef.TxtAsset && loader.SpriteReference.ElemIdx == spriteRef.ElemIdx)
                        SVGAtlas.UpdatePivotHierarchy(gameObj, deltaMovement, 0);
                }
            }
        }

        spriteData.Pivot = newPivot;
        newSprite.name = oldSprite.name;
        EditorUtility.CopySerialized(newSprite, oldSprite);
        SVGUtils.MarkObjectDirty(oldSprite);
        // destroy the temporary sprite
        GameObject.DestroyImmediate(newSprite);
    }

    private void SortingOrdersCompact(SVGAssetInput svgAsset)
    {
        List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
        // get the list of instantiated sprites relative to this atlas generator
        List<GameObject> spritesInstances = new List<GameObject>();
        this.GetSpritesInstances(spritesInstances);
        
        foreach (GameObject gameObj in spritesInstances)
        {
            SVGSpriteLoaderBehaviour spriteLoader = (SVGSpriteLoaderBehaviour)gameObj.GetComponent<SVGSpriteLoaderBehaviour>();
            SVGSpriteRef spriteRef = spriteLoader.SpriteReference;
            // if the sprite belongs to the specified SVG asset input, keep track of it
            if (spriteRef.TxtAsset == svgAsset.TxtAsset)
            {
                SpriteRenderer renderer = (SpriteRenderer)gameObj.GetComponent<SpriteRenderer>();
                if (renderer != null)
                    spriteRenderers.Add(renderer);
            }
        }

        if (spriteRenderers.Count > 0)
        {
            // order the list by current sorting order
            spriteRenderers.Sort(delegate(SpriteRenderer renderer1, SpriteRenderer renderer2) {
                if (renderer1.sortingOrder < renderer2.sortingOrder)
                    return -1;
                if (renderer1.sortingOrder > renderer2.sortingOrder)
                    return 1;
                return 0;
            });

            int j = spriteRenderers.Count;
            for (int i = 0; i < j; ++i)
            {
                SpriteRenderer renderer = spriteRenderers[i];
                int currentOrder = renderer.sortingOrder;
                // isolate high part
                int svgIndex = currentOrder & SPRITES_SORTING_DOCUMENTS_MASK;
                // assign the new order
                renderer.sortingOrder = SVGAtlas.SortingOrderCalc(svgIndex, i);
            }
            svgAsset.InstanceBaseIdx = j;
        }
        else
            // there are no sprite instances relative to the specified SVG, so we can start from 0
            svgAsset.InstanceBaseIdx = 0;
    }

    private void ResetGroupFlags(SVGSpritesList spritesList)
    {
        // now we can unflag sprites
        foreach (SVGSpriteRef spriteRef in spritesList.Sprites)
        {
            // get sprite and its data
            SVGSpriteAssetFile spriteAsset;
            if (this.m_GeneratedSpritesFiles.TryGetValue(spriteRef, out spriteAsset))
            {
                SVGSpriteData spriteData = spriteAsset.SpriteData;
                spriteData.InCurrentInstancesGroup = false;
            }
        }
    }

    private bool InstancesGroupWrap(SVGAssetInput svgAsset, int spritesCount)
    {
        int rangeLo = svgAsset.InstanceBaseIdx;
        int rangeHi = rangeLo + spritesCount;
        return (rangeHi >= SPRITES_SORTING_MAX_INSTANCES) ? true : false;
    }

    private void NextInstancesGroup(SVGAssetInput svgAsset, SVGSpritesList spritesList, int instantiationCount)
    {
        int spritesCount = spritesList.Sprites.Count;

        svgAsset.InstanceBaseIdx += spritesCount;
        if (this.InstancesGroupWrap(svgAsset, spritesCount))
        {
            // try to compact used sorting orders (looping game objects that reference this svg)
            this.SortingOrdersCompact(svgAsset);

            // after compaction, if the instantiation of one or all sprites belonging to the new instances group will wrap
            // we have two options:
            //
            // 1. to instantiate sprites in the normal consecutive way, wrapping aroung SPRITES_SORTING_MAX_INSTANCES: in this case a part of sprites will
            // result (sortingOrder) consistent, but the whole sprites group won't
            //
            // 2. to reset the base index to 0 and generate the sprites according to their natural z-order: in this case the whole sprites group will
            // be (sortingOrder) consistent, but it is not granted to be totally (z)separated from other sprites/instances
            //

            if (this.InstancesGroupWrap(svgAsset, spritesCount))
            {
                svgAsset.InstanceBaseIdx = 0;
                /*
                // option 2
                if (instantiationCount > 1)
                    packedSvg.InstanceBaseIdx = 0;
                // for single sprite instantiation we implicitly use option 1
                */
            }
        }

        // now we can unflag sprites
        this.ResetGroupFlags(spritesList);
    }

    private static int SortingOrderCalc(int svgIndex, int instance)
    {
        svgIndex = svgIndex % SPRITES_SORTING_MAX_DOCUMENTS;
        instance = instance % SPRITES_SORTING_MAX_INSTANCES;

        return ((svgIndex << SPRITES_SORTING_INSTANCES_BITS) + instance);
    }

    private static int SortingOrderCalc(int svgIndex, int instanceBaseIdx, int zOrder)
    {
        return SVGAtlas.SortingOrderCalc(svgIndex, instanceBaseIdx + zOrder);
    }

    private int SortingOrderGenerate(SVGSpriteAssetFile spriteAsset)
    {
        if (spriteAsset != null)
        {
            SVGSpriteRef spriteRef = spriteAsset.SpriteRef;
            SVGSpriteData spriteData = spriteAsset.SpriteData;

            int svgIndex = this.SvgAssetIndexGet(spriteRef.TxtAsset);
            if (svgIndex >= 0)
            {
                SVGSpritesList spritesList;
                SVGAssetInput svgAsset = this.m_SvgList[svgIndex];

                // if needed, advance in the instances group
                if (spriteData.InCurrentInstancesGroup)
                {
                    // get the list of sprites (references) relative to the SVG input asset
                    if (this.m_GeneratedSpritesLists.TryGetValue(svgAsset.TxtAsset.GetInstanceID(), out spritesList))
                        // advance instances group, telling that we are going to instantiate one sprite only
                        this.NextInstancesGroup(svgAsset, spritesList, 1);
                }
                return SVGAtlas.SortingOrderCalc(svgIndex, svgAsset.InstanceBaseIdx, spriteData.ZOrder);
            }
        }
        return -1;
    }

    // recalculate sorting orders of instantiated sprites: changing is due only to SVG index, so the lower part (group + zNatural) is left unchanged
    private void SortingOrdersUpdateSvgIndex()
    {
        // get the list of instantiated sprites relative to this atlas generator
        List<GameObject> spritesInstances = new List<GameObject>();
        this.GetSpritesInstances(spritesInstances);
        
        foreach (GameObject gameObj in spritesInstances)
        {
            SVGSpriteLoaderBehaviour spriteLoader = (SVGSpriteLoaderBehaviour)gameObj.GetComponent<SVGSpriteLoaderBehaviour>();
            SpriteRenderer renderer = (SpriteRenderer)gameObj.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                SVGSpriteRef spriteRef = spriteLoader.SpriteReference;
                int svgIndex = this.SvgAssetIndexGet(spriteRef.TxtAsset);
                if (svgIndex >= 0)
                {
                    int instance = renderer.sortingOrder & SPRITES_SORTING_INSTANCES_MASK;
                    renderer.sortingOrder = SVGAtlas.SortingOrderCalc(svgIndex, instance);
                }
            }
        }
    }

    private GameObject Instantiate(SVGSpriteAssetFile spriteAsset, int sortingOrder)
    {
        SVGSpriteRef spriteRef = spriteAsset.SpriteRef;
        SVGSpriteData spriteData = spriteAsset.SpriteData;
        GameObject gameObj = new GameObject();
        SpriteRenderer renderer = (SpriteRenderer)gameObj.AddComponent<SpriteRenderer>();
        SVGSpriteLoaderBehaviour spriteLoader = (SVGSpriteLoaderBehaviour)gameObj.AddComponent<SVGSpriteLoaderBehaviour>();
        renderer.sprite = spriteData.Sprite;
        renderer.sortingOrder = sortingOrder;
        spriteLoader.Atlas = this;
        spriteLoader.SpriteReference = spriteRef;
        spriteLoader.ResizeOnStart = true;
        gameObj.name = spriteData.Sprite.name;
        spriteData.InCurrentInstancesGroup = true;
        return gameObj;
    }

    private GameObject InstantiateSprite(SVGSpriteAssetFile spriteAsset, Vector3 worldPos, int sortingOrder)
    {
        GameObject gameObj = this.Instantiate(spriteAsset, sortingOrder);
        // assign world position
        gameObj.transform.position = worldPos;
        return gameObj;
    }

    public GameObject InstantiateSprite(SVGSpriteRef spriteRef, Vector3 worldPos)
    {
        SVGSpriteAssetFile spriteAsset;

        if (this.m_GeneratedSpritesFiles.TryGetValue(spriteRef, out spriteAsset))
        {
            int sortingOrder = this.SortingOrderGenerate(spriteAsset);
            GameObject gameObj = this.Instantiate(spriteAsset, sortingOrder);
            // assign world position
            gameObj.transform.position = worldPos;
            return gameObj;
        }
        return null;
    }

    public GameObject InstantiateSprite(SVGSpriteRef spriteRef)
    {
        SVGSpriteAssetFile spriteAsset;

        if (this.m_GeneratedSpritesFiles.TryGetValue(spriteRef, out spriteAsset))
        {
            int sortingOrder = this.SortingOrderGenerate(spriteAsset);
            return this.Instantiate(spriteAsset, sortingOrder);
        }
        return null;
    }

    public GameObject[] InstantiateGroups(SVGAssetInput svgAsset)
    {
        SVGSpritesList spritesList;

        if (svgAsset != null && this.m_GeneratedSpritesLists.TryGetValue(svgAsset.TxtAsset.GetInstanceID(), out spritesList))
        {
            int spritesCount = spritesList.Sprites.Count;
            int svgIndex = this.SvgAssetIndexGet(svgAsset.TxtAsset);
            if (svgIndex >= 0 && spritesCount > 0)
            {
                // list of sprite assets (file) relative to the specified SVG; in this case we can set the right list capacity
                List<SVGSpriteAssetFile> spriteAssets = new List<SVGSpriteAssetFile>(spritesCount);

                bool advanceInstancesGroup = false;
                // now we are sure that at least one valid sprite box exists
                float xMin = float.MaxValue;
                float yMin = float.MaxValue;
                float xMax = float.MinValue;
                float yMax = float.MinValue;

                foreach (SVGSpriteRef spriteRef in spritesList.Sprites)
                {
                    SVGSpriteAssetFile spriteAsset;
                    if (this.m_GeneratedSpritesFiles.TryGetValue(spriteRef, out spriteAsset))
                    {
                        SVGSpriteData spriteData = spriteAsset.SpriteData;
                        Sprite sprite = spriteData.Sprite;
                        //float scl = 1 / spriteData.Scale;
                        float scl = 1;
                        float ox = (float)spriteData.OriginalX;
                        float oy = (float)spriteData.OriginalY;
                        float spriteMinX = ox * scl;
                        float spriteMinY = oy * scl;
                        float spriteMaxX = (ox + sprite.rect.width) * scl;
                        float spriteMaxY = (oy + sprite.rect.height) * scl;

                        // update min corner
                        if (spriteMinX < xMin)
                            xMin = spriteMinX;
                        if (spriteMinY < yMin)
                            yMin = spriteMinY;
                        // update max corner
                        if (spriteMaxX > xMax)
                            xMax = spriteMaxX;
                        if (spriteMaxY > yMax)
                            yMax = spriteMaxY;
                        // if there is a single sprite already instantiated in the current group, we have to advance in the next instances group
                        if (spriteData.InCurrentInstancesGroup)
                            advanceInstancesGroup = true;
                        // keep track of this sprite asset
                        spriteAssets.Add(spriteAsset);
                    }
                }

                if (spriteAssets.Count > 0)
                {
                    // because at least one valid sprite box exists, now we are sure that a valid "global" box has been calculated
                    float centerX = (xMin + xMax) / 2;
                    float centerY = (yMin + yMax) / 2;
                    float boxHeight = yMax - yMin;
                    List<GameObject> instances = new List<GameObject>();

                    if (advanceInstancesGroup)
                        // advance in the instances group, telling that we are going to instantiate N sprites
                        this.NextInstancesGroup(svgAsset, spritesList, spriteAssets.Count);

                    foreach (SVGSpriteAssetFile spriteAsset in spriteAssets)
                    {
                        SVGSpriteData spriteData = spriteAsset.SpriteData;
                        Sprite sprite = spriteData.Sprite;
                        Vector2 pivot = spriteData.Pivot;
                        //float scl = 1 / spriteData.Scale;
                        float scl = 1;
                        float px = (sprite.rect.width * pivot.x + (float)spriteData.OriginalX) * scl - centerX;
                        float py = boxHeight - (sprite.rect.height * (1 - pivot.y) + (float)spriteData.OriginalY) * scl - centerY;
                        Vector2 worldPos = new Vector2(px / SVGBasicAtlas.SPRITE_PIXELS_PER_UNIT, py / SVGBasicAtlas.SPRITE_PIXELS_PER_UNIT);
                        // instantiate the object
                        int sortingOrder = SVGAtlas.SortingOrderCalc(svgIndex, svgAsset.InstanceBaseIdx, spriteData.ZOrder);
                        //instances.Add(this.InstantiateSprite(spriteAsset, worldPos, sortingOrder));
                        GameObject newObj = this.InstantiateSprite(spriteAsset, worldPos, sortingOrder);
                        newObj.transform.localScale = new Vector3(scl, scl, 1);
                        spriteData.InCurrentInstancesGroup = true;
                    }
                    // return the created instances
                    return instances.ToArray();
                }
            }
        }
        return null;
    }

    protected override bool SvgAssetAdd(TextAsset newSvg, int index, bool alreadyExist)
    {
        if (alreadyExist)
        {
            // show warning
            EditorUtility.DisplayDialog("Can't add the same SVG file multiple times!",
                                        string.Format("The list of SVG assets already contains the {0} file.", newSvg.name),
                                        "Ok");
            return false;
        }
        else
        {
            if (this.m_SvgList.Count < SPRITES_SORTING_MAX_DOCUMENTS)
                return true;
            else
            {
                // show warning
                EditorUtility.DisplayDialog("Can't add the SVG file, slots full!",
                                            string.Format("SVG list cannot exceed its maximum capacity of {0} entries. Try to merge some SVG files.", SPRITES_SORTING_MAX_DOCUMENTS),
                                            "Ok");

                return false;
            }
        }
    }

    public override bool SvgAssetMove(SVGAssetInput svgAsset, int toIndex)
    {
        int fromIndex = this.SvgAssetIndexGet(svgAsset.TxtAsset);
        bool moved = this.SvgAssetMove(svgAsset, fromIndex, toIndex);

        if (moved)
        {
            // recalculate sorting orders of instantiated sprites
            this.SortingOrdersUpdateSvgIndex();
        }
        return moved;
    }

    // return true if the atlas needs an update (i.e. a call to UpdateSprites), else false
    protected override string CalcAtlasHash()
    {
        int count = this.m_SvgList.Count;

        if (count > 0)
        {
            // we want the parameters string to come always in front (when sorted)
            string[] hashList = new string[count + 1];
            // parameters string
            string paramsStr = "#*";
            paramsStr += this.m_ReferenceWidth + "-";
            paramsStr += this.m_ReferenceHeight + "-";
            paramsStr += this.m_DeviceTestWidth + "-";
            paramsStr += this.m_DeviceTestHeight + "-";
            paramsStr += this.m_ScaleType + "-";
            paramsStr += this.m_Match + "-";
            paramsStr += this.m_OffsetScale + "-";
            paramsStr += this.m_Pow2Textures + "-";
            paramsStr += this.m_MaxTexturesDimension + "-";
            paramsStr += this.m_SpritesBorder + "-";
            paramsStr += this.m_ClearColor.ToString() + "-";
            paramsStr += this.FullOutputFolder;
            hashList[0] = paramsStr;
            // for each input SVG row we define an "id string"
            for (int i = 0; i < count; ++i)
            {
                hashList [i + 1] = this.m_SvgList [i].Hash ();
            }
            // sort strings, so we can be SVG rows order independent
            Array.Sort(hashList);
            // return MD5 hash
            return SVGBasicAtlas.MD5Calc(String.Join("-", hashList));
        }
        return "";
    }

#endif // UNITY_EDITOR

    // Calculate the scale factor that would be used to generate sprites if the screen would have the specified dimensions
    public float ScaleFactorCalc(int currentScreenWidth, int currentScreenHeight)
    {
        return SVGRuntimeGenerator.ScaleFactorCalc((float)this.m_ReferenceWidth, (float)this.m_ReferenceHeight, currentScreenWidth, currentScreenHeight, this.m_ScaleType, this.m_OffsetScale);
    }

    // Generate a sprite set, according to specified screen dimensions; return true if case of success, else false
    public bool GenerateSprites(int currentScreenWidth, int currentScreenHeight, List<Texture2D> textures, List<KeyValuePair<SVGSpriteRef, SVGSpriteData>> sprites)
    {
        float scale = this.ScaleFactorCalc(currentScreenWidth, currentScreenHeight);
        return this.GenerateSprites(scale, textures, sprites);
    }

    // return true if case of success, else false
    public bool UpdateRuntimeSprites(int currentScreenWidth, int currentScreenHeight, out float scale)
    {
        float newScale = SVGRuntimeGenerator.ScaleFactorCalc((float)this.m_ReferenceWidth, (float)this.m_ReferenceHeight, currentScreenWidth, currentScreenHeight, this.m_ScaleType, this.m_OffsetScale);

        if (Math.Abs(this.m_RuntimeGenerationScale - newScale) > Single.Epsilon)
        {
            scale = newScale;
            return this.UpdateRuntimeSprites(newScale);
        }
        else
        {
            scale = this.m_RuntimeGenerationScale;
            return true;
        }
    }

    void OnEnable()
    {
        this.Initialize();

        if (this.m_ReferenceWidth == 0)
            this.m_ReferenceWidth = (int)SVGAssets.ScreenResolutionWidth;
        
        if (this.m_ReferenceHeight == 0)
            this.m_ReferenceHeight = (int)SVGAssets.ScreenResolutionHeight;

    #if UNITY_EDITOR
        if (this.m_DeviceTestWidth == 0)
            this.m_DeviceTestWidth = this.m_ReferenceWidth;
        
        if (this.m_DeviceTestHeight == 0)
            this.m_DeviceTestHeight = this.m_ReferenceHeight;
    #endif
    }

    public int ReferenceWidth
    {
        get
        {
            return this.m_ReferenceWidth;
        }
        set
        {
            this.m_ReferenceWidth = value;
        #if UNITY_EDITOR
            this.UpdateAtlasHash();
        #endif
        }
    }

    public int ReferenceHeight
    {
        get
        {
            return this.m_ReferenceHeight;
        }
        set
        {
            this.m_ReferenceHeight = value;
        #if UNITY_EDITOR
            this.UpdateAtlasHash();
        #endif
        }
    }

#if UNITY_EDITOR
    public int DeviceTestWidth
    {
        get
        {
            return this.m_DeviceTestWidth;
        }
        set
        {
            this.m_DeviceTestWidth = value;
            this.UpdateAtlasHash();
        }
    }
    
    public int DeviceTestHeight
    {
        get
        {
            return this.m_DeviceTestHeight;
        }
        set
        {
            this.m_DeviceTestHeight = value;
            this.UpdateAtlasHash();
        }
    }
#endif

    public SVGScaleType ScaleType
    {
        get
        {
            return this.m_ScaleType;
        }
        set
        {
            this.m_ScaleType = value;
        #if UNITY_EDITOR
            this.UpdateAtlasHash();
        #endif
        }
    }

    public float Match
    {
        get
        {
            return this.m_Match;
        }
        set
        {
            this.m_Match = value;
        #if UNITY_EDITOR
            this.UpdateAtlasHash();
        #endif
        }
    }

    // Scale adaption
    [SerializeField]
    private int m_ReferenceWidth = 0;
    [SerializeField]
    private int m_ReferenceHeight = 0;
#if UNITY_EDITOR
    [SerializeField]
    private int m_DeviceTestWidth = 0;
    [SerializeField]
    private int m_DeviceTestHeight = 0;
#endif
    [SerializeField]
    private SVGScaleType m_ScaleType = SVGScaleType.MatchWidthOrHeight;
    [SerializeField]
    private float m_Match = 0.5f;

    // number of bits (sortingOrder) dedicated to SVG index
    public const int SPRITES_SORTING_DOCUMENTS_BITS = 4;
    // number of bits (sortingOrder) dedicated to instance index + zOrder
    public const int SPRITES_SORTING_INSTANCES_BITS = (15 - SPRITES_SORTING_DOCUMENTS_BITS);
    // Isolate the sortingOrder high part
    public const int SPRITES_SORTING_DOCUMENTS_MASK = (((1 << SPRITES_SORTING_DOCUMENTS_BITS) - 1) << SPRITES_SORTING_INSTANCES_BITS);
    // Isolate the sortingOrder low part
    public const int SPRITES_SORTING_INSTANCES_MASK = ((1 << SPRITES_SORTING_INSTANCES_BITS) - 1);
    // Max number of SVG inputs/row
    public const int SPRITES_SORTING_MAX_DOCUMENTS = (1 << SPRITES_SORTING_DOCUMENTS_BITS);
    // Max number of sprites instances for each SVG document
    public const int SPRITES_SORTING_MAX_INSTANCES = (1 << SPRITES_SORTING_INSTANCES_BITS);
}
