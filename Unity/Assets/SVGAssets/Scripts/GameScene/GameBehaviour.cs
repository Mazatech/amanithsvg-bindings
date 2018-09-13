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
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
	public Sprite UpdateCardSprite(GameCardBehaviour card)
	{
		if (this.Atlas != null)
		{
			CardType cardType = card.BackSide ? CardType.BackSide : card.AnimalType;
			SVGRuntimeSprite data = this.Atlas.GetSpriteByName(GameCardBehaviour.AnimalSpriteName(cardType));
			// get the sprite, given its name
			if (data != null)
			{
				card.gameObject.GetComponent<SpriteRenderer>().sprite = data.Sprite;
				// keep updated the SVGSpriteLoaderBehaviour component too
				SVGSpriteLoaderBehaviour loader = card.gameObject.GetComponent<SVGSpriteLoaderBehaviour>();
				if (loader != null)
					loader.SpriteReference = data.SpriteReference;
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
				this.Cards[i].GetComponent<BoxCollider2D>().size = sprite.bounds.size;
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
				// assign the new sprites and update colliders
				this.UpdateCardsSprites();
		}
	}

	private void DisposeCards()
	{
		if (this.Camera != null)
		{
			int[] cardsIndexes;
			float cardSlotDimension, rowWorldDimension, columnWorldDimension;
			int slotsPerRow, slotsPerColumn;
			float worldWidth = this.Camera.WorldWidth;
			float worldHeight = this.Camera.WorldHeight;

			if (worldWidth <= worldHeight)
			{
				cardSlotDimension = worldWidth / 3;
				// the real usable dimensions (in world coordinates)
				rowWorldDimension = worldWidth;
				columnWorldDimension = cardSlotDimension * 4;
				// number of card slots in each dimension
				slotsPerRow = 3;
				slotsPerColumn = 4;
				cardsIndexes = GameBehaviour.CARDS_INDEXES_PORTRAIT;
			}
			else
			{
				cardSlotDimension = worldHeight / 3;
				// the real usable dimensions (in world coordinates)
				rowWorldDimension = cardSlotDimension * 4;
				columnWorldDimension = worldHeight;
				// number of card slots in each dimension
				slotsPerRow = 4;
				slotsPerColumn = 3;
				cardsIndexes = GameBehaviour.CARDS_INDEXES_LANDSCAPE;
			}

			float ofsX = (worldWidth - rowWorldDimension) / 2 + (cardSlotDimension / 2) - (worldWidth / 2);
			float ofsY = (worldHeight - columnWorldDimension) / 2 + (cardSlotDimension / 2) - (worldHeight / 2);
			float deltaX = rowWorldDimension / slotsPerRow;
			float deltaY = columnWorldDimension / slotsPerColumn;
			int cardIdx = 0;

			for (int y = 0; y < slotsPerColumn; ++y)
				for (int x = 0; x < slotsPerRow; ++x)
					this.Cards[cardsIndexes[cardIdx++]].transform.position = new Vector3(x * deltaX + ofsX, y * deltaY + ofsY);
		}
	}

	private void OnResize(int newScreenWidth, int newScreenHeight)
	{
		switch (SVGAssets.DeviceOrientation)
		{
			case DeviceOrientation.LandscapeLeft:
			case DeviceOrientation.LandscapeRight:
				break;
			case DeviceOrientation.Portrait:
			case DeviceOrientation.PortraitUpsideDown:
				break;
			case DeviceOrientation.FaceUp:
				break;
			case DeviceOrientation.FaceDown:
				break;
			case DeviceOrientation.Unknown:
				break;
		}

		this.ResizeBackground(newScreenWidth, newScreenHeight);
		this.ResizeCards(newScreenWidth, newScreenHeight);
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

	private void Shuffle(CardType[] array)
	{
		int n = array.Length;
		// Knuth shuffle
		while (n > 1)
		{
			n--;
			int i = this.m_Random.Next(n + 1);
			CardType temp = array[i];
			array[i] = array[n];
			array[n] = temp;
		}
	}

	private IEnumerator ShuffleAnimation()
	{
		this.m_Animating = true;
		for (int i = 0; i < this.Cards.Length; ++i)
			this.Cards[i].GetComponent<Animation>().PlayQueued("cardRotation");
		yield return new WaitForSeconds(2);
		this.m_Animating = false;
	}

	public void StartNewGame()
	{
		int animalTypesCount = (int)CardType.Fox - (int)CardType.Panda + 1;
		int currentAnimal = (int)(UnityEngine.Random.value * (float)animalTypesCount) + (int)CardType.Panda;
		CardType[] animalCouples = new CardType[this.Cards.Length];

		// generate animal couples
		for (int i = 0; i < (this.Cards.Length / 2); ++i)
		{
			animalCouples[i * 2] = animalCouples[i * 2 + 1] = (CardType)(currentAnimal % animalTypesCount);
			currentAnimal++;
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
				this.Background.SVGFile = null;
		}
	}

	private bool GameFinished()
	{
		for (int i = 0; i < this.Cards.Length; ++i)
			if (this.Cards[i].Active)
				return false;
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
		if (this.m_Animating)
			return;
		// card is already in the current selection
		if (card == null || card == this.m_SelectedCard0 || card == this.m_SelectedCard1)
			return;
		// select the first card
		if (this.m_SelectedCard0 == null)
		{
			this.m_SelectedCard0 = card;
			// show card front face
			this.TurnCard(card, false);
			return;
		}
		// select the second card
		if (this.m_SelectedCard1 == null && card != this.m_SelectedCard0)
		{
			this.m_SelectedCard1 = card;
			// show card front face
			this.TurnCard(card, false);
			// if the couple does not match simply turn cards backside, else animate and hide them
			StartCoroutine(this.m_SelectedCard0.AnimalType == this.m_SelectedCard1.AnimalType ? "GoodCouple" : "WrongCouple");
		}
	}

	// Use this for initialization
	void Start()
	{
		this.m_SelectedCard0 = this.m_SelectedCard1 = null;
		// initialize random generator
		this.m_Random = new System.Random(System.Environment.TickCount);
		this.m_Animating = false;
		this.m_BackgroundIndex = this.m_Random.Next();
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
	private System.Random m_Random;
	[NonSerialized]
	private bool m_Animating;
	[NonSerialized]
	private int m_BackgroundIndex;

	private static readonly int[] CARDS_INDEXES_PORTRAIT = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
	private static readonly int[] CARDS_INDEXES_LANDSCAPE = { 9, 6, 3, 0, 10, 7, 4, 1, 11, 8, 5, 2 };
}
