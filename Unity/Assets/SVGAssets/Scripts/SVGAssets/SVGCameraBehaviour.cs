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
#if UNITY_EDITOR
	using UnityEditor;
#endif

[ExecuteInEditMode]
public class SVGCameraBehaviour : MonoBehaviour
{
	private void Resize(int screenWidth, int screenHeight, bool shotEvent)
    {
		// map the camera rectangle to the whole screen; NB: we handle orthographic cameras only
		this.GetComponent<Camera>().aspect = (float)screenWidth / (float)screenHeight;
        this.GetComponent<Camera>().orthographicSize = (screenHeight * this.GetComponent<Camera>().rect.height) / (SVGBasicAtlas.SPRITE_PIXELS_PER_UNIT * 2);
		// keep track of current device dimensions
		this.m_LastScreenWidth = screenWidth;
		this.m_LastScreenHeight = screenHeight;
       	// call OnResize handlers
		if (this.OnResize != null && shotEvent)
			this.OnResize(screenWidth, screenHeight);
    }

	public void Resize(bool shotEvent)
	{
        // update camera aspect ratio and orthographic size
        this.Resize((int)SVGAssets.ScreenResolutionWidth, (int)SVGAssets.ScreenResolutionHeight, shotEvent);
	}

	public float PixelWidth
	{
		// get the camera viewport width, in pixels
		get
		{
			return this.m_LastScreenWidth;
		}
	}
	
	public float PixelHeight
	{
		// get the camera viewport height, in pixels
		get
		{
			return this.m_LastScreenHeight;
		}
	}

	public float WorldWidth
	{
		// get the camera viewport width, in world coordinates
        get
        {
            return this.PixelWidth / SVGBasicAtlas.SPRITE_PIXELS_PER_UNIT;
        }
    }
    
    public float WorldHeight
    {
        // get the camera viewport height, in world coordinates
        get
        {
            return this.PixelHeight / SVGBasicAtlas.SPRITE_PIXELS_PER_UNIT;
        }
    }

    void Start()
    {
		// set the camera so that its viewing volume coincides with the whole device screen (or with the GameView, if inside Editor)
		this.Resize(false);
    }
    
    void Update()
    {
        // get the current screen size
		int curScreenWidth = (int)SVGAssets.ScreenResolutionWidth;
		int curScreenHeight = (int)SVGAssets.ScreenResolutionHeight;

        // if screen size has changed (e.g. device orientation changed), fire the event
		if (curScreenWidth != this.m_LastScreenWidth || curScreenHeight != this.m_LastScreenHeight)
			// update camera aspect ratio and orthographic size
			this.Resize(curScreenWidth, curScreenHeight, Application.isPlaying);
    }

#if UNITY_EDITOR
	// this script works with orthographic cameras only
	private bool RequirementsCheck()
	{
		if (this.GetComponent<Camera>() == null || (!this.GetComponent<Camera>().orthographic))
		{
			EditorUtility.DisplayDialog("Incompatible game object",
			                            string.Format("In order to work properly, the component {0} must be attached to an orthographic camera", this.GetType()),
			                            "Ok");
			DestroyImmediate(this);
			return false;
		}
		return true;
	}

    // Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component the first time.
    // This function is only called in editor mode. Reset is most commonly used to give good default values in the inspector.
    void Reset()
    {
		this.RequirementsCheck();
    }
#endif

    public delegate void OnResizeEvent(int newScreenWidth, int newScreenHeight);
    public event OnResizeEvent OnResize;
    // device screen dimensions
	private int m_LastScreenWidth;
    private int m_LastScreenHeight;
}
