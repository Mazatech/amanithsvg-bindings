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
using System.Collections;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public Sprite UpdateCardSprite(GameCardBehaviour card)
    {
        if (this.Atlas != null)
        {
            GameCardType cardType = card.BackSide ? GameCardType.BackSide : card.AnimalType;
            SVGRuntimeSprite data = this.Atlas.GetSpriteByName(GameCardBehaviour.AnimalSpriteName(cardType));
            // get the sprite, given its name
            if (data != null)
            {
                card.gameObject.GetComponent<SpriteRenderer>().sprite = data.Sprite;
                // keep updated the SVGSpriteLoaderBehaviour component too
                SVGSpriteLoaderBehaviour loader = card.gameObject.GetComponent<SVGSpriteLoaderBehaviour>();
                if (loader != null)
                {
                    loader.SpriteReference = data.SpriteReference;
                }
                return data.Sprite;
            }
        }
        return null;
    }

    private void UpdateCardsSprites()
    {
        // assign the new sprites and update colliders
        for (int i = 0; i < this.Cards.Length; ++i)
        {
            Sprite sprite = this.UpdateCardSprite(this.Cards[i]);
            if (sprite != null)
            {
                this.Cards[i].GetComponent<BoxCollider2D>().size = sprite.bounds.size;
            }
        }
    }

    private void ResizeBackground(int newScreenWidth, int newScreenHeight)
    {
        if (this.Background != null)
        {
            // we want to cover the whole screen
            this.Background.SlicedWidth = newScreenWidth;
            this.Background.SlicedHeight = newScreenHeight;
            // render the background
            this.Background.UpdateBackground(true);
        }
    }

    private void ResizeCards(int newScreenWidth, int newScreenHeight)
    {
        if (this.Atlas != null)
        {
            float scale;
            // update card sprites according to the current screen resolution
            if (this.Atlas.UpdateRuntimeSprites(newScreenWidth, newScreenHeight, out scale))
            {
                // assign the new sprites and update colliders
                this.UpdateCardsSprites();
            }
        }
    }

    private void DisposeCards()
    {
        if (this.Camera != null)
        {
            int[] cardsIndexes;
            int slotsPerRow, slotsPerColumn;
            SVGRuntimeSprite data = this.Atlas.GetSpriteByName(GameCardBehaviour.AnimalSpriteName(GameCardType.BackSide));
            float cardWidth = data.Sprite.bounds.size.x;
            float cardHeight = data.Sprite.bounds.size.y;
            float worldWidth = this.Camera.WorldWidth;
            float worldHeight = this.Camera.WorldHeight;

            if (worldWidth <= worldHeight)
            {
                // number of card slots in each dimension
                slotsPerRow = 3;
                slotsPerColumn = 4;
                cardsIndexes = CARDS_INDEXES_PORTRAIT;
            }
            else
            {
                // number of card slots in each dimension
                slotsPerRow = 4;
                slotsPerColumn = 3;
                cardsIndexes = CARDS_INDEXES_LANDSCAPE;
            }

            // 5% border
            float ofsX = worldWidth * 0.05f;
            float ofsY = worldHeight * 0.05f;
            float horizSeparator = ((worldWidth - (slotsPerRow * cardWidth) - (2.0f * ofsX)) / (slotsPerRow - 1));
            float vertSeparator = ((worldHeight - (slotsPerColumn * cardHeight) - (2.0f * ofsY)) / (slotsPerColumn - 1));
            int cardIdx = 0;

            for (int y = 0; y < slotsPerColumn; ++y)
            {
                for (int x = 0; x < slotsPerRow; ++x)
                {
                    float posX = ofsX + (x * (cardWidth + horizSeparator)) - (worldWidth * 0.5f) + (cardWidth * 0.5f);
                    float posY = ofsY + (y * (cardHeight + vertSeparator)) - (worldHeight * 0.5f) + (cardHeight * 0.5f);
                    this.Cards[cardsIndexes[cardIdx]].transform.position = new Vector3(posX, posY);
                    cardIdx++;
                }
            }
        }
    }

    private void OnResize(int newScreenWidth, int newScreenHeight)
    {
        // resize the background
        this.ResizeBackground(newScreenWidth, newScreenHeight);
        // resize animals sprites
        this.ResizeCards(newScreenWidth, newScreenHeight);
        // rearrange cards on the screen
        this.DisposeCards();
    }

    public void HideCard(GameCardBehaviour card)
    {
        card.Active = false;
        // disable renderer
        card.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        // disable collider
        card.GetComponent<BoxCollider2D>().enabled = false;
    }
    
    public void ShowCard(GameCardBehaviour card)
    {
        card.Active = true;
        // enable renderer
        card.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        // enable collider
        card.GetComponent<BoxCollider2D>().enabled = true;
    }

    private void TurnCard(GameCardBehaviour card, bool backSide)
    {
        card.BackSide = backSide;
        this.UpdateCardSprite(card);
    }

    private void Shuffle(GameCardType[] array)
    {
        System.Random rnd = new System.Random(System.Environment.TickCount);
        int n = array.Length;

        // Knuth shuffle
        while (n > 1)
        {
            n--;
            int i = rnd.Next(n + 1);
            GameCardType temp = array[i];
            array[i] = array[n];
            array[n] = temp;
        }
    }

    private IEnumerator ShuffleAnimation()
    {
        this.m_Animating = true;
        for (int i = 0; i < this.Cards.Length; ++i)
        {
            this.Cards[i].GetComponent<Animation>().PlayQueued("cardRotation");
        }
        yield return new WaitForSeconds(2);
        this.m_Animating = false;
    }

    private void StartNewGame()
    {
        GameCardType[] animalCouples = new GameCardType[this.Cards.Length];
        // start with a random animal
        GameCardType currentAnimal = GameCardBehaviour.RandomAnimal();

        // generate animal couples
        for (int i = 0; i < (this.Cards.Length / 2); ++i)
        {
            animalCouples[i * 2] = currentAnimal;
            animalCouples[(i * 2) + 1] = currentAnimal;
            currentAnimal = GameCardBehaviour.NextAnimal(currentAnimal);
        }
        // shuffle couples
        this.Shuffle(animalCouples);
        // assign cards
        for (int i = 0; i < this.Cards.Length; ++i)
        {
            this.Cards[i].BackSide = true;
            this.Cards[i].AnimalType = animalCouples[i];
            this.ShowCard(this.Cards[i]);
        }

        // select a background
        if (this.Background != null)
        {
            if (this.BackgroundFiles.Length > 0)
            {
                // destroy current texture and sprite
                this.Background.DestroyAll(true);
                // assign a new SVG file
                this.Background.SVGFile = this.BackgroundFiles[this.m_BackgroundIndex % this.BackgroundFiles.Length];
                this.m_BackgroundIndex++;
            }
            else
            {
                this.Background.SVGFile = null;
            }
        }

        // no selection
        this.m_SelectedCard0 = null;
        this.m_SelectedCard1 = null;
    }

    private bool GameFinished()
    {
        for (int i = 0; i < this.Cards.Length; ++i)
        {
            if (this.Cards[i].Active)
            {
                return false;
            }
        }
        // game is completed if all cards are inactive
        return true;
    }

    private IEnumerator WrongCouple()
    {
        yield return new WaitForSeconds(1.5f);
        // show card back face
        this.TurnCard(this.m_SelectedCard0, true);
        this.TurnCard(this.m_SelectedCard1, true);
        this.m_SelectedCard0 = this.m_SelectedCard1 = null;
    }

    private IEnumerator GoodCouple()
    {
        this.m_Animating = true;
        this.m_SelectedCard0.GetComponent<Animation>().PlayQueued("cardRotation");
        this.m_SelectedCard1.GetComponent<Animation>().PlayQueued("cardRotation");
        yield return new WaitForSeconds(2);
        this.m_Animating = false;
        // hide both cards
        this.HideCard(this.m_SelectedCard0);
        this.HideCard(this.m_SelectedCard1);
        this.m_SelectedCard0 = this.m_SelectedCard1 = null;
        // if we have finished, start a new game!
        if (this.GameFinished())
        {
            // start a new game: shuffle the cards deck and change the background
            this.StartNewGame();
            // assign the new sprites and update colliders
            this.UpdateCardsSprites();
            // because we have changed SVG file, we want to be sure that the new background will cover the whole screen
            this.ResizeBackground((int)SVGAssets.ScreenResolutionWidth, (int)SVGAssets.ScreenResolutionHeight);
            // show the "shuffle" animation
            StartCoroutine(this.ShuffleAnimation());
        }
    }

    public void SelectCard(GameCardBehaviour card)
    {
        // avoid selection during animation
        if (!this.m_Animating)
        {
            // card is already in the current selection
            if ((card != this.m_SelectedCard0) && (card != this.m_SelectedCard1))
            {
                // select the first card
                if (this.m_SelectedCard0 == null)
                {
                    this.m_SelectedCard0 = card;
                    // show card front face
                    this.TurnCard(card, false);
                }
                else
                // select the second card
                if ((this.m_SelectedCard1 == null) && (card != this.m_SelectedCard0))
                {
                    this.m_SelectedCard1 = card;
                    // show card front face
                    this.TurnCard(card, false);
                    // if the couple does not match simply turn cards backside, else animate and hide them
                    StartCoroutine(this.m_SelectedCard0.AnimalType == this.m_SelectedCard1.AnimalType ? "GoodCouple" : "WrongCouple");
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        this.m_Animating = false;
        // select a random background
        this.m_BackgroundIndex = System.Environment.TickCount % BackgroundFiles.Length;
        // start a new game
        this.StartNewGame();

        if (this.Camera != null)
        {
            // register ourself for receiving resize events
            this.Camera.OnResize += this.OnResize;
            // now fire a resize event by hand
            this.Camera.Resize(true);
        }
        // show the "shuffle" animation
        StartCoroutine(this.ShuffleAnimation());
    }

#if UNITY_EDITOR
    // Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component the first time.
    // This function is only called in editor mode. Reset is most commonly used to give good default values in the inspector.
    void Reset()
    {
        this.Camera = null;
        this.Background = null;
        this.Atlas = null;
        this.Cards = null;
        this.m_SelectedCard0 = null;
        this.m_SelectedCard1 = null;
        this.m_Animating = false;
    }
#endif
    // the main camera, used to intercept screen resize events
    public SVGCameraBehaviour Camera;
    // the game background
    public SVGBackgroundBehaviour Background;
    // the atlas used to generate animals sprite
    public SVGAtlas Atlas;
    // array of cards
    public GameCardBehaviour[] Cards;
    // array of usable SVG backgrounds
    public TextAsset[] BackgroundFiles;

    [NonSerialized]
    private GameCardBehaviour m_SelectedCard0;
    [NonSerialized]
    private GameCardBehaviour m_SelectedCard1;
    [NonSerialized]
    private bool m_Animating;
    [NonSerialized]
    private int m_BackgroundIndex;

    private static readonly int[] CARDS_INDEXES_PORTRAIT = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
    private static readonly int[] CARDS_INDEXES_LANDSCAPE = { 9, 6, 3, 0, 10, 7, 4, 1, 11, 8, 5, 2 };
}
