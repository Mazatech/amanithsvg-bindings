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
using UnityEngine.UI;
#if UNITY_EDITOR
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;
    using UnityEditor;
    using UnityEditorInternal;
    using System.Reflection;
#endif

public enum SVGUIWidgetType
{
    Image = 0,
    Button = 1,
    //Toggle = 2,
    //Slider = 3,
    //ScrollBar = 4,
    //DropDown = 5,
    InputField = 2,
    Panel = 3
    //ScrollView = 8
}

public class SVGUIAtlas : SVGBasicAtlas
{
#if UNITY_EDITOR

    // get all game objects with an attached SVGUISpriteLoaderBehaviour component
    public void GetSpritesInstances(List<GameObject> spritesInstances)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject gameObj in allObjects)
        {
            // check if the game object is an "SVG sprite" instance of this atlas generator
            if (gameObj.activeInHierarchy)
            {
                SVGUISpriteLoaderBehaviour loader = gameObj.GetComponent<SVGUISpriteLoaderBehaviour>();
                // we must be sure that the loader component must refer to this atlas
                if ((loader != null) && (loader.UIAtlas == this))
                {
                    // add this instance to the output lists
                    spritesInstances.Add(gameObj);
                }
            }
        }
    }

    // get all game objects with an attached SVGUISpriteLoaderBehaviour component that refers a specified sprite asset
    public void GetSpritesInstances(List<GameObject> spritesInstances, SVGSpriteRef spriteRef)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject gameObj in allObjects)
        {
            // check if the game object is an "SVG sprite" instance of this atlas generator
            if (gameObj.activeInHierarchy)
            {
                SVGUISpriteLoaderBehaviour loader = gameObj.GetComponent<SVGUISpriteLoaderBehaviour>();
                // we must be sure that the loader component must refer to this atlas
                if ((loader != null) && (loader.UIAtlas == this) && (loader.SpriteReference.Equals(spriteRef)))
                {
                    // add this instance to the output lists
                    spritesInstances.Add(gameObj);
                }
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
            this.m_SvgListHashOld = "";
            this.m_SvgListHashCurrent = "";
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
                SVGUISpriteLoaderBehaviour spriteLoader = (SVGUISpriteLoaderBehaviour)gameObj.GetComponent<SVGUISpriteLoaderBehaviour>();

                if (spriteLoader.SpriteReference.TxtAsset != null)
                {
                    if (this.m_GeneratedSpritesFiles.TryGetValue(spriteLoader.SpriteReference, out spriteAsset))
                    {
                        // link the new sprite to the renderer
                        Image image = (Image)gameObj.GetComponent<Image>();
                        if (image != null)
                        {
                            SVGSpriteData spriteData = spriteAsset.SpriteData;
                            // assign the new sprite
                            image.sprite = spriteData.Sprite;
                            gameObj.GetComponent<RectTransform>().sizeDelta = new Vector2(spriteData.Sprite.rect.width, spriteData.Sprite.rect.height);
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

    public void UpdateEditorSprites()
    {
        this.UpdateEditorSprites(this.m_CanvasScaleFactor * this.m_OffsetScale);
    }

    public void UpdateBorder(SVGSpriteAssetFile spriteAsset, Vector4 newBorder)
    {
        SVGSpriteData spriteData = spriteAsset.SpriteData;
        Sprite oldSprite = spriteData.Sprite;
        // create a new sprite (same texture, same rectangle, same pivot, different border)
        Sprite newSprite = Sprite.Create(oldSprite.texture, oldSprite.rect, spriteData.Pivot, SVGBasicAtlas.SPRITE_PIXELS_PER_UNIT, 0, SpriteMeshType.FullRect, newBorder);
        spriteData.Border = newBorder;
        newSprite.name = oldSprite.name;
        // serialized copy from the new sprite (i.e. the sprite with updated border) to the old sprite (i.e. the sprite to be updated)
        // NB: we use this technique because there isn't an explicit sprite method that allows the change of border property
        EditorUtility.CopySerialized(newSprite, oldSprite);
        SVGUtils.MarkObjectDirty(oldSprite);
        // destroy the temporary sprite
        GameObject.DestroyImmediate(newSprite);
    }

    public void UpdateBorder(SVGSpriteAssetFile spriteAsset, Vector2 newPivot, Vector4 newBorder)
    {
        SVGSpriteData spriteData = spriteAsset.SpriteData;
        Sprite oldSprite = spriteData.Sprite;
        // create a new sprite (same texture, same rectangle, same pivot, different border)
        Sprite newSprite = Sprite.Create(oldSprite.texture, oldSprite.rect, newPivot, SVGBasicAtlas.SPRITE_PIXELS_PER_UNIT, 0, SpriteMeshType.FullRect, newBorder);
        spriteData.Pivot = newPivot;
        spriteData.Border = newBorder;
        newSprite.name = oldSprite.name;
        // serialized copy from the new sprite (i.e. the sprite with updated border) to the old sprite (i.e. the sprite to be updated)
        // NB: we use this technique because there isn't an explicit sprite method that allows the change of border property
        EditorUtility.CopySerialized(newSprite, oldSprite);
        SVGUtils.MarkObjectDirty(oldSprite);
        // destroy the temporary sprite
        GameObject.DestroyImmediate(newSprite);
    }

    private void PopulateInputField(Canvas canvas, GameObject inputFieldObj, InputField inputField)
    {
        GameObject placeHolderObj = new GameObject();
        placeHolderObj.name = "Placeholder";
        placeHolderObj.layer = LayerMask.NameToLayer("UI");
        Text placeHolder = placeHolderObj.AddComponent<Text>();
        placeHolder.supportRichText = true;
        placeHolder.fontStyle = FontStyle.Italic;
        placeHolder.text = "Enter text...";
        placeHolder.alignment = TextAnchor.MiddleLeft;
        placeHolder.color = new Color(0.196f, 0.196f, 0.196f, 0.5f);
        RectTransform placeHolderRectTransform = placeHolderObj.GetComponent<RectTransform>();
        placeHolderRectTransform.anchorMin = Vector2.zero;
        placeHolderRectTransform.anchorMax = Vector2.one;
        placeHolderRectTransform.SetParent(inputFieldObj.transform);
        placeHolderRectTransform.offsetMin = new Vector2(10, 6);
        placeHolderRectTransform.offsetMax = new Vector2(-10, -7);
        inputField.placeholder = placeHolder;

        GameObject textObj = new GameObject();
        textObj.name = "Text";
        textObj.layer = LayerMask.NameToLayer("UI");
        Text text = textObj.AddComponent<Text>();
        text.supportRichText = false;
        text.alignment = TextAnchor.MiddleLeft;
        text.color = new Color(0.196f, 0.196f, 0.196f, 1.0f);
        RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
        textRectTransform.anchorMin = Vector2.zero;
        textRectTransform.anchorMax = Vector2.one;
        textRectTransform.SetParent(inputFieldObj.transform);
        textRectTransform.offsetMin = new Vector2(10, 6);
        textRectTransform.offsetMax = new Vector2(-10, -7);
        inputField.textComponent = text;
    }

    private GameObject InstantiateWidget(Canvas canvas, SVGSpriteAssetFile spriteAsset, SVGUIWidgetType widgetType)
    {
        Image uiImage;
        InputField inputField = null;
        SVGSpriteRef spriteRef = spriteAsset.SpriteRef;
        SVGSpriteData spriteData = spriteAsset.SpriteData;
        GameObject gameObj = new GameObject();
   
        gameObj.layer = LayerMask.NameToLayer("UI");
        // add Image component; NB: it will add a RectTransform component too
        uiImage = gameObj.AddComponent<Image>();
        uiImage.fillCenter = true;
        uiImage.material = null;
        uiImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        uiImage.sprite = spriteData.Sprite;

        switch (widgetType)
        {
            case SVGUIWidgetType.Button:
                gameObj.AddComponent<Button>();
                break;
            case SVGUIWidgetType.InputField:
                inputField = gameObj.AddComponent<InputField>();
                break;
            default:
                break;
        }

        // get RectTransform component and set size according to the associated sprite
        RectTransform rectTransform = gameObj.GetComponent<RectTransform>();
        rectTransform.SetParent(canvas.transform);

        // attach SVGUISpriteLoaderBehaviour component
        SVGUISpriteLoaderBehaviour spriteLoader = (SVGUISpriteLoaderBehaviour)gameObj.AddComponent<SVGUISpriteLoaderBehaviour>();
        spriteLoader.UIAtlas = this;
        spriteLoader.SpriteReference = spriteRef;
        spriteLoader.ResizeOnStart = true;

        // anchor presets
        if (widgetType == SVGUIWidgetType.Panel) {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }
        else
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(spriteData.Sprite.rect.width, spriteData.Sprite.rect.height);
        }

        // set image type
        uiImage.type = (spriteData.Sprite.border != Vector4.zero) ? Image.Type.Sliced : Image.Type.Simple;

        if (widgetType == SVGUIWidgetType.InputField)
        {
            this.PopulateInputField(canvas, gameObj, inputField);
        }

        gameObj.name = spriteData.Sprite.name;
        gameObj.SetActive(true);
        return gameObj;
    }

    public GameObject InstantiateWidget(Canvas canvas, SVGSpriteRef spriteRef, SVGUIWidgetType widgetType)
    {
        SVGSpriteAssetFile spriteAsset;

        if (this.m_GeneratedSpritesFiles.TryGetValue(spriteRef, out spriteAsset))
        {
            return this.InstantiateWidget(canvas, spriteAsset, widgetType);
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
            return true;
    }

    public override bool SvgAssetMove(SVGAssetInput svgAsset, int toIndex)
    {
        int fromIndex = this.SvgAssetIndexGet(svgAsset.TxtAsset);
        return this.SvgAssetMove(svgAsset, fromIndex, toIndex);
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
            paramsStr += this.m_OffsetScale + "-";
            paramsStr += this.m_Pow2Textures + "-";
            paramsStr += this.m_MaxTexturesDimension + "-";
            paramsStr += this.m_SpritesBorder + "-";
            paramsStr += this.m_ClearColor.ToString() + "-";
            paramsStr += this.FullOutputFolder;
            //paramsStr += "-" + this.m_CanvasScaleFactor.ToString();

            hashList[0] = paramsStr;
            // for each input SVG row we define an "id string"
            for (int i = 0; i < count; ++i)
                hashList[i + 1] = this.m_SvgList[i].Hash();
            // sort strings, so we can be SVG rows order independent
            Array.Sort(hashList);
            // return MD5 hash
            return SVGBasicAtlas.MD5Calc(String.Join("-", hashList));
        }
        return "";
    }

#endif

    void OnEnable()
    {
        this.Initialize();
    }

    public float CanvasScaleFactor
    {
        get
        {
            return this.m_CanvasScaleFactor;
        }
        set
        {
            if (!Application.isPlaying) {
                this.m_CanvasScaleFactor = value;
            #if UNITY_EDITOR
                this.UpdateAtlasHash();
            #endif
            }
        }
    }

    [SerializeField]
    private float m_CanvasScaleFactor = 1.0f;
}
