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

public class OrcBehaviour : MonoBehaviour
{
    private Vector3 CameraPosCalc(Vector3 orcPos)
    {
        // set the camera according to the orc position
        Vector3 cameraPos = new Vector3(orcPos.x, 0, -10);
        float cameraWorldLeft = cameraPos.x - (this.Camera.WorldWidth / 2);
        float cameraWorldRight = cameraPos.x + (this.Camera.WorldWidth / 2);

        // make sure the camera won't go outside the background
        if (cameraWorldLeft < this.Background.WorldLeft)
        {
            cameraPos.x += this.Background.WorldLeft - cameraWorldLeft;
        }
        if (cameraWorldRight > this.Background.WorldRight)
        {
            cameraPos.x -= cameraWorldRight - this.Background.WorldRight;
        }
        return cameraPos;
    }

    private void CameraPosAssign(Vector3 orcPos)
    {
        if ((this.Background != null) && (this.Camera != null))
        {
            // set the camera according to the orc position
            this.Camera.transform.position = (this.Camera.PixelWidth > this.Background.PixelWidth) ? new Vector3(0, 0, -10) : this.CameraPosCalc(orcPos);
        }
    }

    private float BackgroundWalkingLine()
    {
        // walking line is located at ~12% of the background half height, in world coordinates
        return (Background != null) ? (-this.Background.WorldHeight * 0.5f) * 0.12f : 0.0f;
    }

    private void ResetOrcPos()
    {
        Vector3 orcPos = new Vector3(0, this.BackgroundWalkingLine(), 0);
        // move the orc at the walking line
        this.transform.position = orcPos;
        this.CameraPosAssign(orcPos);
    }

    private void WalkAnimation()
    {
        if (this.m_Animator != null)
        {
            this.m_Animator.Play("walking");
        }
    }
    
    private void IdleAnimation()
    {
        if (this.m_Animator != null)
        {
            this.m_Animator.Play("idle");
        }
    }

    private void Move(Vector3 delta)
    {
        // move the orc
        Vector3 orcPos = this.transform.position + delta;
        // get the orc (body) sprite loader
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        float orcBodyWidth = spriteRenderer.sprite.bounds.size.x;
        // orc body pivot is located at 50% of the whole orc sprite, so we can calculate bounds easily
        float orcWorldLeft = orcPos.x - (orcBodyWidth / 2);
        float orcWorldRight = orcPos.x + (orcBodyWidth / 2);
        // make sure the orc won't go outside the background
        if (orcWorldLeft < this.Background.WorldLeft)
        {
            orcPos.x += (this.Background.WorldLeft - orcWorldLeft);
        }
        else
        if (orcWorldRight > this.Background.WorldRight)
        {
            orcPos.x -= (orcWorldRight - this.Background.WorldRight);
        }
        // update the orc position
        this.transform.position = orcPos;
        // set the camera according to the orc position
        this.CameraPosAssign(orcPos);
        this.WalkAnimation();
    }

    private void MoveLeft()
    {
        // flip the orc horizontally
        this.transform.localScale = new Vector3(-1, this.transform.localScale.y, this.transform.localScale.z);
        this.Move(new Vector3(-WALKING_SPEED, 0, 0));
    }
    
    private void MoveRight()
    {
        this.transform.localScale = new Vector3(1, this.transform.localScale.y, this.transform.localScale.z);
        this.Move(new Vector3 (WALKING_SPEED, 0, 0));
    }

    private void ResizeBackground(int newScreenWidth, int newScreenHeight)
    {
        if (this.Background != null)
        {
            // we want to cover the whole screen
            Pair<SVGBackgroundScaleType, int> scaleData = this.Background.CoverFullScreen(newScreenWidth, newScreenHeight);
            this.Background.ScaleAdaption = scaleData.First;
            this.Background.Size = scaleData.Second;
            this.Background.UpdateBackground(false);
        }
    }

    private void ResizeOrcCharacter(int backgroundWidth, int backgroundHeight)
    {
        // get the orc (body) sprite loader
        SVGSpriteLoaderBehaviour spriteLoader = gameObject.GetComponent<SVGSpriteLoaderBehaviour>();
        // update/regenerate all orc sprites; NB: we want to size the orc according to
        // the background sprite (actually the background height)
        if (spriteLoader != null)
        {
            spriteLoader.UpdateSprite(true, backgroundWidth, backgroundHeight);
        }
    }

    private void OnResize(int newScreenWidth, int newScreenHeight)
    {
        // render the background at the right resolution
        this.ResizeBackground(newScreenWidth, newScreenHeight);
        // update/regenerate all orc sprites according to the background dimensions
        this.ResizeOrcCharacter((int)this.Background.PixelWidth, (int)this.Background.PixelHeight);
        // move the orc at the world origin and set the camera according to the orc position
        this.ResetOrcPos();
    }

    void Start()
    {
        // start with the "idle" animation
        this.m_Animator = this.gameObject.GetComponent<Animator>();
        this.IdleAnimation();
        // move the background at world origin and get the reference to its monobehaviour script
        if (this.Background != null)
        {
            this.Background.transform.position = new Vector3(0, 0, 0);
        }
        // register handler for device orientation change
        if (this.Camera != null)
        {
            // register ourself for receiving resize events
            this.Camera.OnResize += this.OnResize;
            // now fire a resize event by hand
            this.Camera.Resize(true);
        }
    }
    
    void LateUpdate()
    {
        if (Input.GetButton("Fire1"))
        {
            Vector3 worldMousePos = this.Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            
            // get the orc (body) sprite loader
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            float orcBodyWidth = spriteRenderer.sprite.bounds.size.x;
            // orc body pivot is located at 50% of the whole orc sprite, so we can calculate bounds easily
            float orcWorldLeft = transform.position.x - (orcBodyWidth / 2);
            float orcWorldRight = transform.position.x + (orcBodyWidth / 2);

            if (worldMousePos.x > orcWorldRight)
            {
                this.MoveRight();
            }
            else
            if (worldMousePos.x < orcWorldLeft)
            {
                this.MoveLeft();
            }
        }
        else
        {
            this.IdleAnimation();
        }
    }

    // the scene camera
    public SVGCameraBehaviour Camera;
    // the background gameobject
    public SVGBackgroundBehaviour Background;
    // the orc animator
    private Animator m_Animator;
    // the walking speed, in world coordinates
    private const float WALKING_SPEED = 0.04f;
}
