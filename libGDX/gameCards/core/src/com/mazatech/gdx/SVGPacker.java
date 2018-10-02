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

package com.mazatech.gdx;

// AmanithSVG
import com.mazatech.svgt.AmanithSVG;
import com.mazatech.svgt.SVGTError;

public class SVGPacker {

    class SVGPackerPage {

        private SVGPackerPage(int index) {

            _index = index;
            build();
        }

        private void build() {

            int[] binInfo = new int[3];

            // get information relative to the surface/page
            AmanithSVG.svgtPackingBinInfo(_index, binInfo);
            // store information relative to the surface/page
            _width = binInfo[0];
            _height = binInfo[1];
            _nativeRectCount = binInfo[2];
            // get native rectangles
            _nativeRects = AmanithSVG.svgtPackingBinRects(_index);
        }

        public int getIndex() {

            return _index;
        }

        public int getWidth() {

            return _width;
        }

        public int getHeight() {

            return _height;
        }

        public int getNativeRectsCount() {

            return _nativeRectCount;
        }

        java.nio.ByteBuffer getNativeRects() {

            return _nativeRects.asReadOnlyBuffer();
        }

        private int _index;
        private int _width;
        private int _height;
        private int _nativeRectCount;
        private java.nio.ByteBuffer _nativeRects;
    }

    class SVGPackerResult {

        private SVGPackerResult() {

            build();
        }

        private void build() {

            int pagesCount = AmanithSVG.svgtPackingBinsCount();

            if (pagesCount > 0) {
                _pages = new SVGPackerPage[pagesCount];
                for (int i = 0; i < pagesCount; ++i) {
                    _pages[i] = new SVGPackerPage(i);
                }
            }
        }

        SVGPackerPage[] getPages() {

            return _pages;
        }

        private SVGPackerPage[] _pages = null;
    }

    // Constructor.
    SVGPacker(float scale, int maxTexturesDimension, int border, boolean pow2Textures) {

        if (scale <= 0) {
            throw new IllegalArgumentException("scale <= 0");
        }
        if (maxTexturesDimension <= 0) {
            throw new IllegalArgumentException("maxTexturesDimension <= 0");
        }
        if (border < 0) {
            throw new IllegalArgumentException("border < 0");
        }

        _scale = scale;
        _maxTexturesDimension = maxTexturesDimension;
        _border = border;
        _pow2Textures = pow2Textures;
        _packing = false;
    }

    SVGTError begin() {

        if (_packing) {
            return SVGTError.StillPacking;
        }
        else {
            SVGTError err = AmanithSVG.svgtPackingBegin(_maxTexturesDimension, _border, _pow2Textures, _scale);
            // check for errors
            if (err == SVGTError.None) {
                _packing = true;
            }
            return err;
        }
    }

    SVGTError add(SVGDocument document, boolean explodeGroup, float scale, int[] info) {

        if (document == null) {
            throw new IllegalArgumentException("document == null");
        }
        if (info == null) {
            throw new IllegalArgumentException("info == null");
        }
        if (info.length < 2) {
            throw new IllegalArgumentException("info parameter must be an array of at least 2 entries");
        }
        else {
            if (!_packing) {
                return SVGTError.NotPacking;
            }
            else {
                // add an SVG document to the current packing task, and get back information about collected bounding boxes
                return AmanithSVG.svgtPackingAdd(document.getHandle(), explodeGroup, scale, info);
                // info[0] = number of collected bounding boxes
                // info[1] = the actual number of packed bounding boxes (boxes whose dimensions exceed the 'maxDimension' value specified to the svgtPackingBegin function, will be discarded)
            }
        }
    }

    SVGPackerResult end(boolean performPacking) {

        SVGTError err;

        if (!_packing) {
            return null;
        }

        // close the current packing task
        if ((err = AmanithSVG.svgtPackingEnd(performPacking)) != SVGTError.None) {
            return null;
        }
        // if requested, close the packing process without doing anything
        if (!performPacking)
            return null;

        return new SVGPackerResult();
    }

    private float _scale;
    private int _maxTexturesDimension;
    private int _border;
    private boolean _pow2Textures;
    private boolean _packing;
}
