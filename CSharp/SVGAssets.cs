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
#if UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE || UNITY_WII || UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_TIZEN || UNITY_TVOS || UNITY_WSA || UNITY_WSA_10_0 || UNITY_WINRT || UNITY_WINRT_10_0 || UNITY_WEBGL || UNITY_FACEBOOK || UNITY_ADS || UNITY_ANALYTICS
    #define UNITY_ENGINE
#endif

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
using System.Runtime.InteropServices;
#if UNITY_ENGINE
    #if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
        using SVGAssetsBridge;
    #endif
    using UnityEngine;
#else
    using System.Drawing;
#endif  // UNITY_ENGINE

public static class AmanithSVG {
#if UNITY_EDITOR
    /* Windows editor will use libAmanithSVG.dll, Max OS X editor will use libAmanithSVG.bundle */
    private const string libName = "libAmanithSVG";
#elif UNITY_STANDALONE_WIN
    /* Windows uses libAmanithSVG.dll */
    private const string libName = "libAmanithSVG";
#elif UNITY_STANDALONE_OSX
    /* Mac OS X uses libAmanithSVG.bundle */
    private const string libName = "libAmanithSVG";
#elif UNITY_STANDALONE_LINUX
    /* Linux uses libAmanithSVG.so please note that plugin name should not include the prefix ('lib') nor the extension ('.so') of the filename */
    private const string libName = "AmanithSVG";
#elif UNITY_IPHONE
    /* On iOS, everything gets built into one big binary, so "__Internal" is the name of the library to use */
    private const string libName = "__Internal";
#elif UNITY_ANDROID
    /* Android uses libAmanithSVG.so please note that plugin name should not include the prefix ('lib') nor the extension ('.so') of the filename */
    private const string libName = "AmanithSVG";
#else
    private const string libName = "libAmanithSVG";
#endif

    /* Invalid handle. */
    public const uint SVGT_INVALID_HANDLE                   = 0;

    /* SVGTboolean */
    public const int SVGT_FALSE                             = 0;
    public const int SVGT_TRUE                              = 1;

    /*
        SVGTAspectRatioAlign

        Alignment indicates whether to force uniform scaling and, if so, the alignment method to use in case the aspect ratio of the source
        viewport doesn't match the aspect ratio of the destination (drawing surface) viewport.
    */

    /*
        Do not force uniform scaling.
        Scale the graphic content of the given element non-uniformly if necessary such that
        the element's bounding box exactly matches the viewport rectangle.
        NB: in this case, the <meetOrSlice> value is ignored.
    */
    public const int SVGT_ASPECT_RATIO_ALIGN_NONE           = 0;

    /*
        Force uniform scaling.
        Align the <min-x> of the source viewport with the smallest x value of the destination (drawing surface) viewport.
        Align the <min-y> of the source viewport with the smallest y value of the destination (drawing surface) viewport.
    */
    public const int SVGT_ASPECT_RATIO_ALIGN_XMINYMIN       = 1;

    /*
        Force uniform scaling.
        Align the <mid-x> of the source viewport with the midpoint x value of the destination (drawing surface) viewport.
        Align the <min-y> of the source viewport with the smallest y value of the destination (drawing surface) viewport.
    */
    public const int SVGT_ASPECT_RATIO_ALIGN_XMIDYMIN       = 2;

    /*
        Force uniform scaling.
        Align the <max-x> of the source viewport with the maximum x value of the destination (drawing surface) viewport.
        Align the <min-y> of the source viewport with the smallest y value of the destination (drawing surface) viewport.
    */
    public const int SVGT_ASPECT_RATIO_ALIGN_XMAXYMIN       = 3;

    /*
        Force uniform scaling.
        Align the <min-x> of the source viewport with the smallest x value of the destination (drawing surface) viewport.
        Align the <mid-y> of the source viewport with the midpoint y value of the destination (drawing surface) viewport.
    */
    public const int SVGT_ASPECT_RATIO_ALIGN_XMINYMID       = 4;

    /*
        Force uniform scaling.
        Align the <mid-x> of the source viewport with the midpoint x value of the destination (drawing surface) viewport.
        Align the <mid-y> of the source viewport with the midpoint y value of the destination (drawing surface) viewport.
    */
    public const int SVGT_ASPECT_RATIO_ALIGN_XMIDYMID       = 5;

    /*
        Force uniform scaling.
        Align the <max-x> of the source viewport with the maximum x value of the destination (drawing surface) viewport.
        Align the <mid-y> of the source viewport with the midpoint y value of the destination (drawing surface) viewport.
    */
    public const int SVGT_ASPECT_RATIO_ALIGN_XMAXYMID       = 6;

    /*
        Force uniform scaling.
        Align the <min-x> of the source viewport with the smallest x value of the destination (drawing surface) viewport.
        Align the <max-y> of the source viewport with the maximum y value of the destination (drawing surface) viewport.
    */
    public const int SVGT_ASPECT_RATIO_ALIGN_XMINYMAX       = 7;

    /*
        Force uniform scaling.
        Align the <mid-x> of the source viewport with the midpoint x value of the destination (drawing surface) viewport.
        Align the <max-y> of the source viewport with the maximum y value of the destination (drawing surface) viewport.
    */
    public const int SVGT_ASPECT_RATIO_ALIGN_XMIDYMAX       = 8;

    /*
        Force uniform scaling.
        Align the <max-x> of the source viewport with the maximum x value of the destination (drawing surface) viewport.
        Align the <max-y> of the source viewport with the maximum y value of the destination (drawing surface) viewport.
    */
    public const int SVGT_ASPECT_RATIO_ALIGN_XMAXYMAX      = 9;


    /* SVGTAspectRatioMeetOrSlice */
    /*
        Scale the graphic such that:
        - aspect ratio is preserved
        - the entire viewBox is visible within the viewport
        - the viewBox is scaled up as much as possible, while still meeting the other criteria

        In this case, if the aspect ratio of the graphic does not match the viewport, some of the viewport will
        extend beyond the bounds of the viewBox (i.e., the area into which the viewBox will draw will be smaller
        than the viewport).
    */
    public const int SVGT_ASPECT_RATIO_MEET                 = 0;

    /*
        Scale the graphic such that:
        - aspect ratio is preserved
        - the entire viewport is covered by the viewBox
        - the viewBox is scaled down as much as possible, while still meeting the other criteria
        
        In this case, if the aspect ratio of the viewBox does not match the viewport, some of the viewBox will
        extend beyond the bounds of the viewport (i.e., the area into which the viewBox will draw is larger
        than the viewport).
    */
    public const int SVGT_ASPECT_RATIO_SLICE                = 1;

    /* SVGTErrorCode */
    public const int SVGT_NO_ERROR                          = 0;
    // it indicates that the library has not previously been initialized through the svgtInit() function
    public const int SVGT_NOT_INITIALIZED_ERROR             = 1;
    public const int SVGT_BAD_HANDLE_ERROR                  = 2;
    public const int SVGT_ILLEGAL_ARGUMENT_ERROR            = 3;
    public const int SVGT_OUT_OF_MEMORY_ERROR               = 4;
    public const int SVGT_PARSER_ERROR                      = 5;
    // returned when the library detects that outermost element is not an <svg> element or there is circular dependency (usually generated by <use> elements)
    public const int SVGT_INVALID_SVG_ERROR                 = 6;
    public const int SVGT_STILL_PACKING_ERROR               = 7;
    public const int SVGT_NOT_PACKING_ERROR                 = 8;
    public const int SVGT_UNKNOWN_ERROR                     = 9;

    /* SVGTRenderingQuality */    
    public const int SVGT_RENDERING_QUALITY_NONANTIALIASED  = 0;
    public const int SVGT_RENDERING_QUALITY_FASTER          = 1;
    public const int SVGT_RENDERING_QUALITY_BETTER          = 2;

    /* SVGTStringID */
    public const int SVGT_VENDOR                            = 1;
    public const int SVGT_VERSION                           = 2;

    /* Packed rectangle */
    [StructLayout(LayoutKind.Sequential)]  
    public struct SVGTPackedRect
    {
        // 'id' attribute, NULL if not present.
        public System.IntPtr elemName;
        // Original rectangle corner.
        public int originalX;
        public int originalY;
        // Rectangle corner position.
        public int x;
        public int y;
        // Rectangle dimensions.
        public int width;
        public int height;
        // SVG document handle.
        public uint docHandle;
        // 0 for the whole SVG, else the element (tree) index.
        public uint elemIdx;
        // Z-order.
        public int zOrder;
        // The used destination viewport width (induced by packing scale factor).
        public float dstViewportWidth;
        // The used destination viewport height (induced by packing scale factor).
        public float dstViewportHeight;
    };

    /*
        Initialize the library.

        It returns SVGT_NO_ERROR if the operation was completed successfully, else an error code.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtInit(uint screenWidth,
                               uint screenHeight,
                               float dpi)
    {
        return SVGAssetsBridge.API.svgtInit(screenWidth, screenHeight, dpi);
    }
#else
    [DllImport(libName)]
    public static extern int svgtInit(uint screenWidth,
                                      uint screenHeight,
                                      float dpi);
#endif

    /*
        Destroy the library, freeing all allocated resources.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static void svgtDone()
    {
        SVGAssetsBridge.API.svgtDone();
    }
#else
    [DllImport(libName)]
    public static extern void svgtDone();
#endif

    /*
        Get the maximum dimension allowed for drawing surfaces.

        This is the maximum valid value that can be specified as 'width' and 'height' for the svgtSurfaceCreate and svgtSurfaceResize functions.
        Bigger values are silently clamped to it.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static uint svgtSurfaceMaxDimension()
    {
        return SVGAssetsBridge.API.svgtSurfaceMaxDimension();
    }
#else
    [DllImport(libName)]
    public static extern uint svgtSurfaceMaxDimension();
#endif

    /*
        Create a new drawing surface, specifying its dimensions in pixels.
        Specified width and height must be greater than zero.

        Return SVGT_INVALID_HANDLE in case of errors, else a valid drawing surface handle.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static uint svgtSurfaceCreate(uint width,
                                         uint height)
    {
        return SVGAssetsBridge.API.svgtSurfaceCreate(width, height);
    }
#else
    [DllImport(libName)]
    public static extern uint svgtSurfaceCreate(uint width,
                                                uint height);
#endif

    /*
        Destroy a previously created drawing surface.

        It returns SVGT_NO_ERROR if the operation was completed successfully, else an error code.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtSurfaceDestroy(uint surface)
    {
        return SVGAssetsBridge.API.svgtSurfaceDestroy(surface);
    }
#else
    [DllImport(libName)]
    public static extern int svgtSurfaceDestroy(uint surface);
#endif

    /*
        Resize a drawing surface, specifying new dimensions in pixels.
        Specified newWidth and newHeight must be greater than zero.

        After resizing, the surface viewport will be reset to the whole surface (see svgtSurfaceViewportGet / svgtSurfaceViewportSet).

        It returns SVGT_NO_ERROR if the operation was completed successfully, else an error code.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtSurfaceResize(uint surface,
                                        uint newWidth,
                                        uint newHeight)
    {
        return SVGAssetsBridge.API.svgtSurfaceResize(surface, newWidth, newHeight);
    }
#else
    [DllImport(libName)]
    public static extern int svgtSurfaceResize(uint surface,
                                               uint newWidth,
                                               uint newHeight);
#endif

    /*
        Get width dimension (in pixels), of the specified drawing surface.
        If the specified surface handle is not valid, 0 is returned.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static uint svgtSurfaceWidth(uint surface)
    {
        return SVGAssetsBridge.API.svgtSurfaceWidth(surface);
    }
#else
    [DllImport(libName)]
    public static extern uint svgtSurfaceWidth(uint surface);
#endif

    /*
        Get height dimension (in pixels), of the specified drawing surface.
        If the specified surface handle is not valid, 0 is returned.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static uint svgtSurfaceHeight(uint surface)
    {
        return SVGAssetsBridge.API.svgtSurfaceHeight(surface);
    }
#else
    [DllImport(libName)]
    public static extern uint svgtSurfaceHeight(uint surface);
#endif

    /*
        Get access to the drawing surface pixels.
        If the specified surface handle is not valid, NULL is returned.

        Please use this function to access surface pixels for read-only purposes (e.g. blit the surface
        on the screen, according to the platform graphic subsystem, upload pixels into a GPU texture, and so on).
        Writing or modifying surface pixels by hand is still possible, but not advisable.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static System.IntPtr svgtSurfacePixels(uint surface)
    {
        return (System.IntPtr)SVGAssetsBridge.API.svgtSurfacePixels(surface);
    }
#else
    [DllImport(libName)]
    public static extern System.IntPtr svgtSurfacePixels(uint surface);
#endif

    /*
        Copy drawing surface content into the specified pixels buffer.
        This method is a shortcut for the following copy operation:

            MemCopy(dstPixels32, svgtSurfacePixels(surface), svgtSurfaceWidth(surface) * svgtSurfaceHeight(surface) * 4)

        This function is useful for managed environments (e.g. C#, Unity, Java, Android), where the use of a direct pixels
        access (i.e. svgtSurfacePixels) is not advisable nor comfortable.

        This function returns:
        - SVGT_BAD_HANDLE_ERROR if the specified surface handle is not valid
        - SVGT_ILLEGAL_ARGUMENT_ERROR if 'dstPixels32' pointer is NULL or if it's not properly aligned
        - SVGT_NO_ERROR if the operation was completed successfully
    */

#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtSurfaceCopy(uint surface,
                                      System.IntPtr dstPixels32,
                                      int redBlueSwap,
                                      int dilateEdgesFix)
    {
        return SVGAssetsBridge.API.svgtSurfaceCopy(surface, dstPixels32.ToInt64(), redBlueSwap, dilateEdgesFix);
    }
#else
    [DllImport(libName)]
    public static extern int svgtSurfaceCopy(uint surface,
                                             System.IntPtr dstPixels32,
                                             int redBlueSwap,
                                             int dilateEdgesFix);
#endif

    /*
        Associate a native "hardware" texture to a drawing surface, setting parameters for the copy&destroy.

        'nativeTexturePtr' must be a valid native "hardware" texture (e.g. GLuint texture "name" on OpenGL/OpenGL ES, IDirect3DBaseTexture9 on D3D9,
        ID3D11Resource on D3D11, on Metal the id<MTLTexture> pointer).

        'nativeTextureWidth' and 'nativeTextureHeight' must be greater than zero.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtSurfaceTexturePtrSet(uint surface,
                                               System.IntPtr nativeTexturePtr,
                                               uint nativeTextureWidth,
                                               uint nativeTextureHeight,
                                               int nativeTextureIsBGRA,
                                               int dilateEdgesFix)
    {
        return SVGAssetsBridge.API.svgtSurfaceTexturePtrSet(surface, nativeTexturePtr.ToInt64(), nativeTextureWidth, nativeTextureHeight, nativeTextureIsBGRA, dilateEdgesFix);
    }
#else
    [DllImport(libName)]
    public static extern int svgtSurfaceTexturePtrSet(uint surface,
                                                      System.IntPtr nativeTexturePtr,
                                                      uint nativeTextureWidth,
                                                      uint nativeTextureHeight,
                                                      int nativeTextureIsBGRA,
                                                      int dilateEdgesFix);
#endif

    /*
        Get the native code callback to queue for Unity's renderer to invoke.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static System.IntPtr svgtSurfaceTextureCopyAndDestroyFuncGet()
    {
        return (System.IntPtr)(SVGAssetsBridge.API.svgtSurfaceTextureCopyAndDestroyFuncGet());
    }
#else
    [DllImport(libName)]
    public static extern System.IntPtr svgtSurfaceTextureCopyAndDestroyFuncGet();
#endif


    /*
        Get current destination viewport (i.e. a drawing surface rectangular area), where to map the source document viewport.

        The 'viewport' parameter must be an array of (at least) 4 float entries, it will be filled with:
        - viewport[0] = top/left x
        - viewport[1] = top/left y
        - viewport[2] = width
        - viewport[3] = height

        This function returns:
        - SVGT_BAD_HANDLE_ERROR if specified surface handle is not valid
        - SVGT_ILLEGAL_ARGUMENT_ERROR if 'viewport' pointer is NULL or if it's not properly aligned
        - SVGT_NO_ERROR if the operation was completed successfully
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtSurfaceViewportGet(uint surface,
                                             float[] viewport)
    {
        return SVGAssetsBridge.API.svgtSurfaceViewportGet(surface, viewport);
    }
#else
    [DllImport(libName)]
    public static extern int svgtSurfaceViewportGet(uint surface,
                                                    float[] viewport);
#endif

    /*
        Set destination viewport (i.e. a drawing surface rectangular area), where to map the source document viewport.

        The 'viewport' parameter must be an array of (at least) 4 float entries, it must contain:
        - viewport[0] = top/left x
        - viewport[1] = top/left y
        - viewport[2] = width
        - viewport[3] = height

        The combined use of svgtDocViewportSet and svgtSurfaceViewportSet induces a transformation matrix, that will be used
        to draw the whole SVG document. The induced matrix grants that the document viewport is mapped onto the surface
        viewport (respecting the specified alignment): all SVG content will be drawn accordingly.

        This function returns:
        - SVGT_BAD_HANDLE_ERROR if specified surface handle is not valid
        - SVGT_ILLEGAL_ARGUMENT_ERROR if 'viewport' pointer is NULL or if it's not properly aligned
        - SVGT_ILLEGAL_ARGUMENT_ERROR if specified viewport width or height are less than or equal zero
        - SVGT_NO_ERROR if the operation was completed successfully

        NB: floating-point values of NaN are treated as 0, values of +Infinity and -Infinity are clamped to the largest and smallest available float values.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtSurfaceViewportSet(uint surface,
                                             float[] viewport)
    {
        return SVGAssetsBridge.API.svgtSurfaceViewportSet(surface, viewport);
    }
#else
    [DllImport(libName)]
    public static extern int svgtSurfaceViewportSet(uint surface,
                                                    float[] viewport);
#endif

    /*
        Create and load an SVG document, specifying the whole xml string.

        Return SVGT_INVALID_HANDLE in case of errors, else a valid document handle.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static uint svgtDocCreate(string xmlText)
    {
        uint svgDoc = SVGT_INVALID_HANDLE;
    #if UNITY_WP8
        if (xmlText == null)
            return SVGT_INVALID_HANDLE;
        // convert the string to C/ansi format
        byte[] utf8Buffer = new byte[xmlText.Length + 1];
        int count = System.Text.UTF8Encoding.UTF8.GetBytes(xmlText, 0, xmlText.Length, utf8Buffer, 0);
        // append the final '\0', in order to be fully compatible with C strings
        utf8Buffer[xmlText.Length] = 0;
        // get the temporary buffer pointer
        GCHandle handle = GCHandle.Alloc(utf8Buffer, GCHandleType.Pinned);
        try
        {
            System.IntPtr cStr = handle.AddrOfPinnedObject();
            if (cStr != System.IntPtr.Zero)
                svgDoc = SVGAssetsBridge.API.svgtDocCreate(cStr.ToInt64());
        }
        finally
        {
            // free the pinned array handle
            if (handle.IsAllocated)
                handle.Free();
        }
    #else
        // convert the string to C/ansi format and get the pointer
        System.IntPtr cStr = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(xmlText);

        if (cStr != System.IntPtr.Zero)
        {
            svgDoc = SVGAssetsBridge.API.svgtDocCreate(cStr.ToInt64());
            System.Runtime.InteropServices.Marshal.FreeHGlobal(cStr);
        }
    #endif
        return svgDoc;
    }
#else
    [DllImport(libName)]
    public static extern uint svgtDocCreate(string xmlText);
#endif

    /*
        Destroy a previously created SVG document.

        It returns SVGT_NO_ERROR if the operation was completed successfully, else an error code.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtDocDestroy(uint svgDoc)
    {
        return SVGAssetsBridge.API.svgtDocDestroy(svgDoc);
    }
#else
    [DllImport(libName)]
    public static extern int svgtDocDestroy(uint svgDoc);
#endif

    /*
        SVG content itself optionally can provide information about the appropriate viewport region for
        the content via the 'width' and 'height' XML attributes on the outermost <svg> element.
        Use this function to get the suggested viewport width, in pixels.

        It returns -1 (i.e. an invalid width) in the following cases:
        - the library has not previously been initialized through the svgtInit() function
        - outermost element is not an <svg> element
        - outermost <svg> element doesn't have a 'width' attribute specified
        - outermost <svg> element has a 'width' attribute specified in relative coordinates units (i.e. em, ex, % percentage)
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static float svgtDocWidth(uint svgDoc)
    {
        return SVGAssetsBridge.API.svgtDocWidth(svgDoc);
    }
#else
    [DllImport(libName)]
    public static extern float svgtDocWidth(uint svgDoc);
#endif

    /*
        SVG content itself optionally can provide information about the appropriate viewport region for
        the content via the 'width' and 'height' XML attributes on the outermost <svg> element.
        Use this function to get the suggested viewport height, in pixels.

        It returns -1 (i.e. an invalid height) in the following cases:
        - the library has not previously been initialized through the svgtInit() function
        - outermost element is not an <svg> element
        - outermost <svg> element doesn't have a 'height' attribute specified
        - outermost <svg> element has a 'height' attribute specified in relative coordinates units (i.e. em, ex, % percentage)
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static float svgtDocHeight(uint svgDoc)
    {
        return SVGAssetsBridge.API.svgtDocHeight(svgDoc);
    }
#else
    [DllImport(libName)]
    public static extern float svgtDocHeight(uint svgDoc);
#endif

    /*
        Get the document (logical) viewport to map onto the destination (drawing surface) viewport.
        When an SVG document has been created through the svgtDocCreate function, the initial value
        of its viewport is equal to the 'viewBox' attribute present in the outermost <svg> element.
        If such element does not contain the viewBox attribute, SVGT_NO_ERROR is returned and viewport
        array will be filled with zeros.

        The 'viewport' parameter must be an array of (at least) 4 float entries, it will be filled with:
        - viewport[0] = top/left x
        - viewport[1] = top/left y
        - viewport[2] = width
        - viewport[3] = height

        This function returns:
        - SVGT_BAD_HANDLE_ERROR if specified document handle is not valid
        - SVGT_ILLEGAL_ARGUMENT_ERROR if 'viewport' pointer is NULL or if it's not properly aligned
        - SVGT_NO_ERROR if the operation was completed successfully
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtDocViewportGet(uint svgDoc,
                                         float[] viewport)
    {
        return SVGAssetsBridge.API.svgtDocViewportGet(svgDoc, viewport);
    }
#else
    [DllImport(libName)]
    public static extern int svgtDocViewportGet(uint svgDoc,
                                                float[] viewport);
#endif

    /*
        Set the document (logical) viewport to map onto the destination (drawing surface) viewport.

        The 'viewport' parameter must be an array of (at least) 4 float entries, it must contain:
        - viewport[0] = top/left x
        - viewport[1] = top/left y
        - viewport[2] = width
        - viewport[3] = height

        This function returns:
        - SVGT_BAD_HANDLE_ERROR if specified document handle is not valid
        - SVGT_ILLEGAL_ARGUMENT_ERROR if 'viewport' pointer is NULL or if it's not properly aligned
        - SVGT_ILLEGAL_ARGUMENT_ERROR if specified viewport width or height are less than or equal zero
        - SVGT_NO_ERROR if the operation was completed successfully

        NB: floating-point values of NaN are treated as 0, values of +Infinity and -Infinity are clamped to the largest and smallest available float values.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtDocViewportSet(uint svgDoc,
                                         float[] viewport)
    {
        return SVGAssetsBridge.API.svgtDocViewportSet(svgDoc, viewport);
    }
#else
    [DllImport(libName)]
    public static extern int svgtDocViewportSet(uint svgDoc,
                                                float[] viewport);
#endif

    /*
        Get the document alignment.
        The alignment parameter indicates whether to force uniform scaling and, if so, the alignment method to use in case
        the aspect ratio of the document viewport doesn't match the aspect ratio of the surface viewport.

        The 'values' parameter must be an array of (at least) 2 unsigned integers entries, it will be filled with:
        - values[0] = alignment (see SVGTAspectRatioAlign)
        - values[1] = meetOrSlice (see SVGTAspectRatioMeetOrSlice)

        This function returns:
        - SVGT_BAD_HANDLE_ERROR if specified document handle is not valid
        - SVGT_ILLEGAL_ARGUMENT_ERROR if 'values' pointer is NULL or if it's not properly aligned
        - SVGT_NO_ERROR if the operation was completed successfully
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtDocViewportAlignmentGet(uint svgDoc,
                                                  uint[] values)
    {
        return SVGAssetsBridge.API.svgtDocViewportAlignmentGet(svgDoc, values);
    }
#else
    [DllImport(libName)]
    public static extern int svgtDocViewportAlignmentGet(uint svgDoc,
                                                         uint[] values);
#endif

    /*
        Set the document alignment.
        The alignment parameter indicates whether to force uniform scaling and, if so, the alignment method to use in case
        the aspect ratio of the document viewport doesn't match the aspect ratio of the surface viewport.

        The 'values' parameter must be an array of (at least) 2 unsigned integers entries, it must contain:
        - values[0] = alignment (see SVGTAspectRatioAlign)
        - values[1] = meetOrSlice (see SVGTAspectRatioMeetOrSlice)

        This function returns:
        - SVGT_BAD_HANDLE_ERROR if specified document handle is not valid
        - SVGT_ILLEGAL_ARGUMENT_ERROR if 'values' pointer is NULL or if it's not properly aligned
        - SVGT_ILLEGAL_ARGUMENT_ERROR if specified alignment is not a valid SVGTAspectRatioAlign value
        - SVGT_ILLEGAL_ARGUMENT_ERROR if specified meetOrSlice is not a valid SVGTAspectRatioMeetOrSlice value
        - SVGT_NO_ERROR if the operation was completed successfully
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtDocViewportAlignmentSet(uint svgDoc,
                                                  uint[] values)
    {
        return SVGAssetsBridge.API.svgtDocViewportAlignmentSet(svgDoc, values);
    }
#else
    [DllImport(libName)]
    public static extern int svgtDocViewportAlignmentSet(uint svgDoc,
                                                         uint[] values);
#endif

    /*
        Draw an SVG document, on the specified drawing surface.
        If the specified SVG document is SVGT_INVALID_HANDLE, the drawing surface is cleared (or not) according to the current
        settings (see svgtClearColor and svgtClearPerform), and nothing else is drawn.

        It returns SVGT_NO_ERROR if the operation was completed successfully, else an error code.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtDocDraw(uint svgDoc,
                                  uint surface,
                                  uint renderingQuality)
    {
        return SVGAssetsBridge.API.svgtDocDraw(svgDoc, surface, renderingQuality);
    }
#else
    [DllImport(libName)]
    public static extern int svgtDocDraw(uint svgDoc,
                                         uint surface,
                                         uint renderingQuality);
#endif

    /*
        Set the clear color (i.e. the color used to clear the whole drawing surface).
        Each color component must be a number between 0 and 1. Values outside this range
        will be clamped.

        It returns SVGT_NO_ERROR if the operation was completed successfully, else an error code.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtClearColor(float r,
                                     float g,
                                     float b,
                                     float a)
    {
        return SVGAssetsBridge.API.svgtClearColor(r, g, b, a);
    }
#else
    [DllImport(libName)]
    public static extern int svgtClearColor(float r,
                                            float g,
                                            float b,
                                            float a);
#endif

    /*
        Specify if the whole drawing surface must be cleared by the svgtDocDraw function, before to draw the SVG document.

        It returns SVGT_NO_ERROR if the operation was completed successfully, else an error code.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtClearPerform(int doClear)
    {
        return SVGAssetsBridge.API.svgtClearPerform(doClear);
    }
#else
    [DllImport(libName)]
    public static extern int svgtClearPerform(int doClear);
#endif

    /*
        Map a point, expressed in the document viewport system, into the surface viewport.
        The transformation will be performed according to the current document viewport (see svgtDocViewportGet) and the
        current surface viewport (see svgtSurfaceViewportGet).

        The 'dst' parameter must be an array of (at least) 2 float entries, it will be filled with:
        - dst[0] = transformed x
        - dst[1] = transformed y

        This function returns:
        - SVGT_BAD_HANDLE_ERROR if specified document (or surface) handle is not valid
        - SVGT_ILLEGAL_ARGUMENT_ERROR if 'dst' pointer is NULL or if it's not properly aligned
        - SVGT_NO_ERROR if the operation was completed successfully

        NB: floating-point values of NaN are treated as 0, values of +Infinity and -Infinity are clamped to the largest and smallest available float values.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtPointMap(uint svgDoc,
                                   uint surface,
                                   float x,
                                   float y,
                                   float[] dst)
    {
        return SVGAssetsBridge.API.svgtPointMap(svgDoc, surface, x, y, dst);
    }
#else
    [DllImport(libName)]
    public static extern int svgtPointMap(uint svgDoc,
                                          uint surface,
                                          float x,
                                          float y,
                                          float[] dst);
#endif

    /*!
        Start a packing task: one or more SVG documents will be collected and packed into bins, for the generation of atlases.

        Every collected SVG document/element will be packed into rectangular bins, whose dimensions won't exceed the specified 'maxDimension', in pixels.
        If SVGT_TRUE, 'pow2Bins' will force bins to have power-of-two dimensions.
        Each rectangle will be separated from the others by the specified 'border', in pixels.
        The specified 'scale' factor will be applied to all collected SVG documents/elements, in order to realize resolution-independent atlases.

        This function returns:
        - SVGT_STILL_PACKING_ERROR if a current packing task is still open
        - SVGT_ILLEGAL_ARGUMENT_ERROR if specified 'maxDimension' is 0
        - SVGT_ILLEGAL_ARGUMENT_ERROR if 'pow2Bins' is SVGT_TRUE and the specified 'maxDimension' is not a power-of-two number
        - SVGT_ILLEGAL_ARGUMENT_ERROR if specified 'border' itself would exceed the specified 'maxDimension' (border must allow a packable region of at least one pixel)
        - SVGT_ILLEGAL_ARGUMENT_ERROR if specified 'scale' factor is less than or equal 0
        - SVGT_NO_ERROR if the operation was completed successfully
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtPackingBegin(uint maxDimension,
                                       uint border,
                                       int pow2Bins,
                                       float scale)
    {
        return SVGAssetsBridge.API.svgtPackingBegin(maxDimension, border, pow2Bins, scale);
    }
#else
    [DllImport(libName)]
    public static extern int svgtPackingBegin(uint maxDimension,
                                              uint border,
                                              int pow2Bins,
                                              float scale);
#endif

    /*!
        Add an SVG document to the current packing task.

        If SVGT_TRUE, 'explodeGroups' tells the packer to not pack the whole SVG document, but instead to pack each first-level element separately.
        The additional 'scale' is used to adjust the document content to the other documents involved in the current packing process.

        The 'info' parameter will return some useful information, it must be an array of (at least) 2 entries and it will be filled with:
        - info[0] = number of collected bounding boxes
        - info[1] = the actual number of packed bounding boxes (boxes whose dimensions exceed the 'maxDimension' value specified to the svgtPackingBegin function, will be discarded)
        
        This function returns:
        - SVGT_NOT_PACKING_ERROR if there isn't a currently open packing task
        - SVGT_BAD_HANDLE_ERROR if specified document handle is not valid
        - SVGT_ILLEGAL_ARGUMENT_ERROR if 'info' pointer is NULL or if it's not properly aligned
        - SVGT_NO_ERROR if the operation was completed successfully
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtPackingAdd(uint svgDoc,
                                     int explodeGroups,
                                     float scale,
                                     uint[] info)
    {
        return SVGAssetsBridge.API.svgtPackingAdd(svgDoc, explodeGroups, scale, info);
    }
#else
    [DllImport(libName)]
    public static extern int svgtPackingAdd(uint svgDoc,
                                            int explodeGroups,
                                            float scale,
                                            uint[] info);
#endif

    /*!
        Close the current packing task and, if specified, perform the real packing algorithm.

        All collected SVG documents/elements (actually their bounding boxes) are packed into bins for later use (i.e. atlases generation).
        After calling this function, the application could use svgtPackingBinsCount, svgtPackingBinInfo and svgtPackingDraw in order to
        get information about the resulted packed elements and draw them.

        This function returns:
        - SVGT_NOT_PACKING_ERROR if there isn't a currently open packing task
        - SVGT_NO_ERROR if the operation was completed successfully
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtPackingEnd(int performPacking)
    {
        return SVGAssetsBridge.API.svgtPackingEnd(performPacking);
    }
#else
    [DllImport(libName)]
    public static extern int svgtPackingEnd(int performPacking);
#endif

    /*!
        Return the number of generated bins from the last packing task.
        
        This function returns a negative number in case of errors (e.g. if the current packing task has not been previously closed by a call to svgtPackingEnd).
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtPackingBinsCount()
    {
        return SVGAssetsBridge.API.svgtPackingBinsCount();
    }
#else
    [DllImport(libName)]
    public static extern int svgtPackingBinsCount();
#endif

    /*!
        Return information about the specified bin.

        The requested bin is selected by its index; the 'binInfo' parameter must be an array of (at least) 3 entries, it will be filled with:
        - binInfo[0] = bin width, in pixels
        - binInfo[1] = bin height, in pixels
        - binInfo[2] = number of packed rectangles inside the bin

        This function returns:
        - SVGT_STILL_PACKING_ERROR if a current packing task is still open
        - SVGT_ILLEGAL_ARGUMENT_ERROR if specified 'binIdx' is not valid (must be >= 0 and less than the value returned by svgtPackingBinsCount function)
        - SVGT_ILLEGAL_ARGUMENT_ERROR if 'binInfo' pointer is NULL or if it's not properly aligned
        - SVGT_NO_ERROR if the operation was completed successfully
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtPackingBinInfo(uint binIdx,
                                         uint[] binInfo)
    {
        return SVGAssetsBridge.API.svgtPackingBinInfo(binIdx, binInfo);
    }
#else
    [DllImport(libName)]
    public static extern int svgtPackingBinInfo(uint binIdx,
                                                uint[] binInfo);
#endif

    /*!
        Get access to packed rectangles, relative to a specified bin.

        The specified 'binIdx' must be >= 0 and less than the value returned by svgtPackingBinsCount function, else a NULL pointer will be returned.
        The returned pointer contains an array of packed rectangles, whose number is equal to the one gotten through the svgtPackingBinInfo function.
        The use case for which this function was created, it's to copy and cache the result of a packing process; then when needed (e.g. requested by
        the application), the rectangles can be drawn using the svgtPackingRectsDraw function.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static System.IntPtr svgtPackingBinRects(uint binIdx)
    {
        return (System.IntPtr)SVGAssetsBridge.API.svgtPackingBinRects(binIdx);
    }
#else
    [DllImport(libName)]
    public static extern System.IntPtr svgtPackingBinRects(uint binIdx);
#endif

    /*!
        Draw a set of packed SVG documents/elements over the specified drawing surface.

        The drawing surface is cleared (or not) according to the current settings (see svgtClearColor and svgtClearPerform).
        After calling svgtPackingEnd, the application could use this function in order to draw packed elements before to start another
        packing task with svgtPackingBegin.

        This function returns:
        - SVGT_STILL_PACKING_ERROR if a current packing task is still open
        - SVGT_ILLEGAL_ARGUMENT_ERROR if specified 'binIdx' is not valid (must be >= 0 and less than the value returned by svgtPackingBinsCount function)
        - SVGT_ILLEGAL_ARGUMENT_ERROR if specified 'startRectIdx', along with 'rectsCount', identifies an invalid range of rectangles; defined:

            maxCount = binInfo[2] (see svgtPackingBinInfo)
            endRectIdx = 'startRectIdx' + 'rectsCount' - 1

        it must be ensured that 'startRectIdx' < maxCount and 'endRectIdx' < maxCount, else SVGT_ILLEGAL_ARGUMENT_ERROR is returned.

        - SVGT_BAD_HANDLE_ERROR if specified surface handle is not valid
        - SVGT_NO_ERROR if the operation was completed successfully
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtPackingDraw(uint binIdx,
                                      uint startRectIdx,
                                      uint rectsCount,
                                      uint surface,
                                      uint renderingQuality)
    {
        return SVGAssetsBridge.API.svgtPackingDraw(binIdx, startRectIdx, rectsCount, surface, renderingQuality);
    }
#else
    [DllImport(libName)]
    public static extern int svgtPackingDraw(uint binIdx,
                                             uint startRectIdx,
                                             uint rectsCount,
                                             uint surface,
                                             uint renderingQuality);
#endif

    /*!
        Draw a set of packed SVG documents/elements over the specified drawing surface.

        The drawing surface is cleared (or not) according to the current settings (see svgtClearColor and svgtClearPerform).
        The specified rectangles MUST NOT point to the memory returned by svgtPackingBinRects.

        This function returns:
        - SVGT_ILLEGAL_ARGUMENT_ERROR if 'rects' pointer is NULL or if it's not properly aligned
        - SVGT_BAD_HANDLE_ERROR if specified surface handle is not valid
        - SVGT_NO_ERROR if the operation was completed successfully
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static int svgtPackingRectsDraw(System.IntPtr rects,
                                           uint rectsCount,
                                           uint surface,
                                           uint renderingQuality)
    {
        return SVGAssetsBridge.API.svgtPackingRectsDraw(rects, rectsCount, surface, renderingQuality);
    }
#else
    [DllImport(libName)]
    public static extern int svgtPackingRectsDraw(System.IntPtr rects,
                                                  uint rectsCount,
                                                  uint surface,
                                                  uint renderingQuality);
#endif

    /*
        Get renderer and version information.
    */
#if (UNITY_WP8 || UNITY_WP_8_1) && !UNITY_EDITOR
    public static System.IntPtr svgtGetString(uint name)
    {
        return (System.IntPtr)SVGAssetsBridge.API.svgtGetString(name);
    }
#else
    [DllImport(libName)]
    public static extern System.IntPtr svgtGetString(uint name);
#endif

    /*
        Get the description of the specified error code.
    */
    public static string svgtErrorDesc(int errorCode)
    {
        switch (errorCode)
        {
            case SVGT_NO_ERROR:
                return "";
            case SVGT_NOT_INITIALIZED_ERROR:
                return "AmanithSVG library has not been initialized (see svgtInit)";
            case SVGT_BAD_HANDLE_ERROR:
                return "Bad handle";
            case SVGT_ILLEGAL_ARGUMENT_ERROR:
                return "Illegal argument";
            case SVGT_OUT_OF_MEMORY_ERROR:
                return "Out of memory";
            case SVGT_PARSER_ERROR:
                return "Parser detected an invalid xml (element or attribute)";
            case SVGT_INVALID_SVG_ERROR:
                return "Outermost element is not an <svg>, or there is a circular dependency (generated by <use> elements)";
            case SVGT_STILL_PACKING_ERROR:
                return "A packing task is still open/running";
            case SVGT_NOT_PACKING_ERROR:
                return "There is not an open/running packing task";
            default:
                return "Unknown error";
        }
    }

    /*
        Log an error message.
    */
    public static void svgtErrorLog(string msgPrefix,
                                    int errorCode)
    {
#if UNITY_ENGINE
        UnityEngine.Debug.Log(msgPrefix + svgtErrorDesc(errorCode));
#else
        Console.WriteLine(msgPrefix + svgtErrorDesc(errorCode));
#endif
    }
}

/*
    Simple color class.
*/
public class SVGColor
{
    // Constructor.
    public SVGColor()
    {
        _r = 1.0f;
        _g = 1.0f;
        _b = 1.0f;
        _a = 0.0f;
    }

    // Set constructor.
    public SVGColor(float r, float g, float b) : this(r, g, b, 1.0f)
    {
    }

    // Set constructor.
    public SVGColor(float r, float g, float b, float a)
    {
        // clamp each component in the [0; 1] range
        _r = (r < 0.0f) ? 0.0f : (r > 1.0f) ? 1.0f : r;
        _g = (g < 0.0f) ? 0.0f : (g > 1.0f) ? 1.0f : g;
        _b = (b < 0.0f) ? 0.0f : (b > 1.0f) ? 1.0f : b;
        _a = (a < 0.0f) ? 0.0f : (a > 1.0f) ? 1.0f : a;
    }

    // Red component (read only).
    public float Red
    {
        get
        {
            return _r;
        }
    }

    // Green component (read only).
    public float Green
    {
        get
        {
            return _g;
        }
    }

    // Blue component (read only).
    public float Blue
    {
        get
        {
            return _b;
        }
    }

    // Alpha component (read only).
    public float Alpha
    {
        get
        {
            return _a;
        }
    }

    // Red component.
    private float _r;
    // Green component.
    private float _g;
    // Blue component.
    private float _b;
    // Alpha component.
    private float _a;

    public static readonly SVGColor Aliceblue = new SVGColor(0.941f, 0.973f, 1.000f);
    public static readonly SVGColor Antiquewhite = new SVGColor(0.980f, 0.922f, 0.843f);
    public static readonly SVGColor Aqua = new SVGColor(0.000f, 1.000f, 1.000f);
    public static readonly SVGColor Aquamarine = new SVGColor(0.498f, 1.000f, 0.831f);
    public static readonly SVGColor Azure = new SVGColor(0.941f, 1.000f, 1.000f);
    public static readonly SVGColor Beige = new SVGColor(0.961f, 0.961f, 0.863f);
    public static readonly SVGColor Bisque = new SVGColor(1.000f, 0.894f, 0.769f);
    public static readonly SVGColor Black = new SVGColor(0.000f, 0.000f, 0.000f);
    public static readonly SVGColor Blanchedalmond = new SVGColor(1.000f, 0.922f, 0.804f);
    public static readonly SVGColor Blueviolet = new SVGColor(0.541f, 0.169f, 0.886f);
    public static readonly SVGColor Brown = new SVGColor(0.647f, 0.165f, 0.165f);
    public static readonly SVGColor Burlywood = new SVGColor(0.871f, 0.722f, 0.529f);
    public static readonly SVGColor Cadetblue = new SVGColor(0.373f, 0.620f, 0.627f);
    public static readonly SVGColor Chartreuse = new SVGColor(0.498f, 1.000f, 0.000f);
    public static readonly SVGColor Chocolate = new SVGColor(0.824f, 0.412f, 0.118f);
    public static readonly SVGColor Coral = new SVGColor(1.000f, 0.498f, 0.314f);
    public static readonly SVGColor Cornflowerblue = new SVGColor(0.392f, 0.584f, 0.929f);
    public static readonly SVGColor Cornsilk = new SVGColor(1.000f, 0.973f, 0.863f);
    public static readonly SVGColor Crimson = new SVGColor(0.863f, 0.078f, 0.235f);
    public static readonly SVGColor Cyan = new SVGColor(0.000f, 1.000f, 1.000f);
    public static readonly SVGColor Darkblue = new SVGColor(0.000f, 0.000f, 0.545f);
    public static readonly SVGColor Darkcyan = new SVGColor(0.000f, 0.545f, 0.545f);
    public static readonly SVGColor Darkgoldenrod = new SVGColor(0.722f, 0.525f, 0.043f);
    public static readonly SVGColor Darkgray = new SVGColor(0.663f, 0.663f, 0.663f);
    public static readonly SVGColor Darkgreen = new SVGColor(0.000f, 0.392f, 0.000f);
    public static readonly SVGColor Darkgrey = new SVGColor(0.663f, 0.663f, 0.663f);
    public static readonly SVGColor Darkkhaki = new SVGColor(0.741f, 0.718f, 0.420f);
    public static readonly SVGColor Darkmagenta = new SVGColor(0.545f, 0.000f, 0.545f);
    public static readonly SVGColor Darkolivegreen = new SVGColor(0.333f, 0.420f, 0.184f);
    public static readonly SVGColor Darkorange = new SVGColor(1.000f, 0.549f, 0.000f);
    public static readonly SVGColor Darkorchid = new SVGColor(0.600f, 0.196f, 0.800f);
    public static readonly SVGColor Darkred = new SVGColor(0.545f, 0.000f, 0.000f);
    public static readonly SVGColor Darksalmon = new SVGColor(0.914f, 0.588f, 0.478f);
    public static readonly SVGColor Darkseagreen = new SVGColor(0.561f, 0.737f, 0.561f);
    public static readonly SVGColor Darkslateblue = new SVGColor(0.282f, 0.239f, 0.545f);
    public static readonly SVGColor Darkslategray = new SVGColor(0.184f, 0.310f, 0.310f);
    public static readonly SVGColor Darkslategrey = new SVGColor(0.184f, 0.310f, 0.310f);
    public static readonly SVGColor Darkturquoise = new SVGColor(0.000f, 0.808f, 0.820f);
    public static readonly SVGColor Darkviolet = new SVGColor(0.580f, 0.000f, 0.827f);
    public static readonly SVGColor Deeppink = new SVGColor(1.000f, 0.078f, 0.576f);
    public static readonly SVGColor Deepskyblue = new SVGColor(0.000f, 0.749f, 1.000f);
    public static readonly SVGColor Dimgray = new SVGColor(0.412f, 0.412f, 0.412f);
    public static readonly SVGColor Dimgrey = new SVGColor(0.412f, 0.412f, 0.412f);
    public static readonly SVGColor Dodgerblue = new SVGColor(0.118f, 0.565f, 1.000f);
    public static readonly SVGColor Firebrick = new SVGColor(0.698f, 0.133f, 0.133f);
    public static readonly SVGColor Floralwhite = new SVGColor(1.000f, 0.980f, 0.941f);
    public static readonly SVGColor Forestgreen = new SVGColor(0.133f, 0.545f, 0.133f);
    public static readonly SVGColor Fuchsia = new SVGColor(1.000f, 0.000f, 1.000f);
    public static readonly SVGColor Gainsboro = new SVGColor(0.863f, 0.863f, 0.863f);
    public static readonly SVGColor Ghostwhite = new SVGColor(0.973f, 0.973f, 1.000f);
    public static readonly SVGColor Gold = new SVGColor(1.000f, 0.843f, 0.000f);
    public static readonly SVGColor Goldenrod = new SVGColor(0.855f, 0.647f, 0.125f);
    public static readonly SVGColor Gray = new SVGColor(0.502f, 0.502f, 0.502f);
    public static readonly SVGColor Greenyellow = new SVGColor(0.678f, 1.000f, 0.184f);
    public static readonly SVGColor Grey = new SVGColor(0.502f, 0.502f, 0.502f);
    public static readonly SVGColor Honeydew = new SVGColor(0.941f, 1.000f, 0.941f);
    public static readonly SVGColor Hotpink = new SVGColor(1.000f, 0.412f, 0.706f);
    public static readonly SVGColor Indianred = new SVGColor(0.804f, 0.361f, 0.361f);
    public static readonly SVGColor Indigo = new SVGColor(0.294f, 0.000f, 0.510f);
    public static readonly SVGColor Ivory = new SVGColor(1.000f, 1.000f, 0.941f);
    public static readonly SVGColor Khaki = new SVGColor(0.941f, 0.902f, 0.549f);
    public static readonly SVGColor Lavender = new SVGColor(0.902f, 0.902f, 0.980f);
    public static readonly SVGColor Lavenderblush = new SVGColor(1.000f, 0.941f, 0.961f);
    public static readonly SVGColor Lawngreen = new SVGColor(0.486f, 0.988f, 0.000f);
    public static readonly SVGColor Lemonchiffon = new SVGColor(1.000f, 0.980f, 0.804f);
    public static readonly SVGColor Lightblue = new SVGColor(0.678f, 0.847f, 0.902f);
    public static readonly SVGColor Lightcoral = new SVGColor(0.941f, 0.502f, 0.502f);
    public static readonly SVGColor Lightcyan = new SVGColor(0.878f, 1.000f, 1.000f);
    public static readonly SVGColor Lightgoldenrodyellow = new SVGColor(0.980f, 0.980f, 0.824f);
    public static readonly SVGColor Lightgray = new SVGColor(0.827f, 0.827f, 0.827f);
    public static readonly SVGColor Lightgreen = new SVGColor(0.565f, 0.933f, 0.565f);
    public static readonly SVGColor Lightgrey = new SVGColor(0.827f, 0.827f, 0.827f);
    public static readonly SVGColor Lightpink = new SVGColor(1.000f, 0.714f, 0.757f);
    public static readonly SVGColor Lightsalmon = new SVGColor(1.000f, 0.627f, 0.478f);
    public static readonly SVGColor Lightseagreen = new SVGColor(0.125f, 0.698f, 0.667f);
    public static readonly SVGColor Lightskyblue = new SVGColor(0.529f, 0.808f, 0.980f);
    public static readonly SVGColor Lightslategray = new SVGColor(0.467f, 0.533f, 0.600f);
    public static readonly SVGColor Lightslategrey = new SVGColor(0.467f, 0.533f, 0.600f);
    public static readonly SVGColor Lightsteelblue = new SVGColor(0.690f, 0.769f, 0.871f);
    public static readonly SVGColor Lightyellow = new SVGColor(1.000f, 1.000f, 0.878f);
    public static readonly SVGColor Lime = new SVGColor(0.000f, 1.000f, 0.000f);
    public static readonly SVGColor Limegreen = new SVGColor(0.196f, 0.804f, 0.196f);
    public static readonly SVGColor Linen = new SVGColor(0.980f, 0.941f, 0.902f);
    public static readonly SVGColor Magenta = new SVGColor(1.000f, 0.000f, 1.000f);
    public static readonly SVGColor Maroon = new SVGColor(0.502f, 0.000f, 0.000f);
    public static readonly SVGColor Mediumaquamarine = new SVGColor(0.400f, 0.804f, 0.667f);
    public static readonly SVGColor Mediumblue = new SVGColor(0.000f, 0.000f, 0.804f);
    public static readonly SVGColor Mediumorchid = new SVGColor(0.729f, 0.333f, 0.827f);
    public static readonly SVGColor Mediumpurple = new SVGColor(0.576f, 0.439f, 0.859f);
    public static readonly SVGColor Mediumseagreen = new SVGColor(0.235f, 0.702f, 0.443f);
    public static readonly SVGColor Mediumslateblue = new SVGColor(0.482f, 0.408f, 0.933f);
    public static readonly SVGColor Mediumspringgreen = new SVGColor(0.000f, 0.980f, 0.604f);
    public static readonly SVGColor Mediumturquoise = new SVGColor(0.282f, 0.820f, 0.800f);
    public static readonly SVGColor Mediumvioletred = new SVGColor(0.780f, 0.082f, 0.522f);
    public static readonly SVGColor Midnightblue = new SVGColor(0.098f, 0.098f, 0.439f);
    public static readonly SVGColor Mintcream = new SVGColor(0.961f, 1.000f, 0.980f);
    public static readonly SVGColor Mistyrose = new SVGColor(1.000f, 0.894f, 0.882f);
    public static readonly SVGColor Moccasin = new SVGColor(1.000f, 0.894f, 0.710f);
    public static readonly SVGColor Navajowhite = new SVGColor(1.000f, 0.871f, 0.678f);
    public static readonly SVGColor Navy = new SVGColor(0.000f, 0.000f, 0.502f);
    public static readonly SVGColor Oldlace = new SVGColor(0.992f, 0.961f, 0.902f);
    public static readonly SVGColor Olive = new SVGColor(0.502f, 0.502f, 0.000f);
    public static readonly SVGColor Olivedrab = new SVGColor(0.420f, 0.557f, 0.137f);
    public static readonly SVGColor Orange = new SVGColor(1.000f, 0.647f, 0.000f);
    public static readonly SVGColor Orangered = new SVGColor(1.000f, 0.271f, 0.000f);
    public static readonly SVGColor Orchid = new SVGColor(0.855f, 0.439f, 0.839f);
    public static readonly SVGColor Palegoldenrod = new SVGColor(0.933f, 0.910f, 0.667f);
    public static readonly SVGColor Palegreen = new SVGColor(0.596f, 0.984f, 0.596f);
    public static readonly SVGColor Paleturquoise = new SVGColor(0.686f, 0.933f, 0.933f);
    public static readonly SVGColor Palevioletred = new SVGColor(0.859f, 0.439f, 0.576f);
    public static readonly SVGColor Papayawhip = new SVGColor(1.000f, 0.937f, 0.835f);
    public static readonly SVGColor Peachpuff = new SVGColor(1.000f, 0.855f, 0.725f);
    public static readonly SVGColor Peru = new SVGColor(0.804f, 0.522f, 0.247f);
    public static readonly SVGColor Pink = new SVGColor(1.000f, 0.753f, 0.796f);
    public static readonly SVGColor Plum = new SVGColor(0.867f, 0.627f, 0.867f);
    public static readonly SVGColor Powderblue = new SVGColor(0.690f, 0.878f, 0.902f);
    public static readonly SVGColor Purple = new SVGColor(0.502f, 0.000f, 0.502f);
    public static readonly SVGColor Rosybrown = new SVGColor(0.737f, 0.561f, 0.561f);
    public static readonly SVGColor Royalblue = new SVGColor(0.255f, 0.412f, 0.882f);
    public static readonly SVGColor Saddlebrown = new SVGColor(0.545f, 0.271f, 0.075f);
    public static readonly SVGColor Salmon = new SVGColor(0.980f, 0.502f, 0.447f);
    public static readonly SVGColor Sandybrown = new SVGColor(0.957f, 0.643f, 0.376f);
    public static readonly SVGColor Seagreen = new SVGColor(0.180f, 0.545f, 0.341f);
    public static readonly SVGColor Seashell = new SVGColor(1.000f, 0.961f, 0.933f);
    public static readonly SVGColor Sienna = new SVGColor(0.627f, 0.322f, 0.176f);
    public static readonly SVGColor Silver = new SVGColor(0.753f, 0.753f, 0.753f);
    public static readonly SVGColor Skyblue = new SVGColor(0.529f, 0.808f, 0.922f);
    public static readonly SVGColor Slateblue = new SVGColor(0.416f, 0.353f, 0.804f);
    public static readonly SVGColor Slategray = new SVGColor(0.439f, 0.502f, 0.565f);
    public static readonly SVGColor Slategrey = new SVGColor(0.439f, 0.502f, 0.565f);
    public static readonly SVGColor Snow = new SVGColor(1.000f, 0.980f, 0.980f);
    public static readonly SVGColor Springgreen = new SVGColor(0.000f, 1.000f, 0.498f);
    public static readonly SVGColor Steelblue = new SVGColor(0.275f, 0.510f, 0.706f);
    public static readonly SVGColor Tan = new SVGColor(0.824f, 0.706f, 0.549f);
    public static readonly SVGColor Teal = new SVGColor(0.000f, 0.502f, 0.502f);
    public static readonly SVGColor Thistle = new SVGColor(0.847f, 0.749f, 0.847f);
    public static readonly SVGColor Tomato = new SVGColor(1.000f, 0.388f, 0.278f);
    public static readonly SVGColor Turquoise = new SVGColor(0.251f, 0.878f, 0.816f);
    public static readonly SVGColor Violet = new SVGColor(0.933f, 0.510f, 0.933f);
    public static readonly SVGColor Wheat = new SVGColor(0.961f, 0.871f, 0.702f);
    public static readonly SVGColor White = new SVGColor(1.000f, 1.000f, 1.000f);
    public static readonly SVGColor Whitesmoke = new SVGColor(0.961f, 0.961f, 0.961f);
    public static readonly SVGColor Yellow = new SVGColor(1.000f, 1.000f, 0.000f);
    public static readonly SVGColor Yellowgreen = new SVGColor(0.604f, 0.804f, 0.196f);
    public static readonly SVGColor Clear = new SVGColor(0.0f, 0.0f, 0.0f, 0.0f);
}

public class SVGPoint
{
    // Constructor.
    public SVGPoint()
    {
        this._x = 0.0f;
        this._y = 0.0f;
    }

    // Set constructor.
    public SVGPoint(float x, float y)
    {
        this._x = x;
        this._y = y;
    }

    // Abscissa.
    public float X
    {
        get
        {
            return this._x;
        }
    }

    // Ordinate.
    public float Y
    {
        get
        {
            return this._y;
        }
    }

    private float _x;
    private float _y;
}

/*
    SVG viewport.

    A viewport represents a rectangular area, specified by its top/left corner, a width and an height.
    The positive x-axis points towards the right, the positive y-axis points down.
*/
public class SVGViewport
{
    // Constructor.
    public SVGViewport()
    {
        this._x = 0.0f;
        this._y = 0.0f;
        this._width = 0.0f;
        this._height = 0.0f;
        this._changed = true;
    }

    // Set constructor.
    public SVGViewport(float x, float y, float width, float height)
    {
        this._x = x;
        this._y = y;
        this._width = (width < 0.0f) ? 0.0f : width;
        this._height = (height < 0.0f) ? 0.0f : height;
        this._changed = true;
    }

    // Top/left corner, abscissa.
    public float X
    {
        get
        {
            return this._x;
        }
        set
        {
            this._x = value;
            this._changed = true;
        }
    }

    // Top/left corner, ordinate.
    public float Y
    {
        get
        {
            return this._y;
        }
        set
        {
            this._y = value;
            this._changed = true;
        }
    }

    // Viewport width.
    public float Width
    {
        get
        {
            return this._width;
        }
        set
        {
            this._width = (value < 0.0f) ? 0.0f : value;
            this._changed = true;
        }
    }

    // Viewport height.
    public float Height
    {
        get
        {
            return this._height;
        }
        set
        {
            this._height = (value < 0.0f) ? 0.0f : value;
            this._changed = true;
        }
    }

    internal bool Changed
    {
        get
        {
            return this._changed;
        }
        set
        {
            this._changed = value;
        }
    }

    // Top/left corner, x.
    private float _x;
    // Top/left corner, y.
    private float _y;
    // Viewport width.
    private float _width;
    // Viewport height.
    private float _height;
    // Keep track if some parameter has been changed.
    private bool _changed;
}

public enum SVGAlign
{
    /*
        SVGAlign

        Alignment indicates whether to force uniform scaling and, if so, the alignment method to use in case the aspect ratio of the source
        viewport doesn't match the aspect ratio of the destination (drawing surface) viewport.
    */

    /*
        Do not force uniform scaling.
        Scale the graphic content of the given element non-uniformly if necessary such that
        the element's bounding box exactly matches the viewport rectangle.
        NB: in this case, the <meetOrSlice> value is ignored.
    */
    None = AmanithSVG.SVGT_ASPECT_RATIO_ALIGN_NONE,

    /*
        Force uniform scaling.
        Align the <min-x> of the source viewport with the smallest x value of the destination (drawing surface) viewport.
        Align the <min-y> of the source viewport with the smallest y value of the destination (drawing surface) viewport.
    */
    XMinYMin = AmanithSVG.SVGT_ASPECT_RATIO_ALIGN_XMINYMIN,

    /*
        Force uniform scaling.
        Align the <mid-x> of the source viewport with the midpoint x value of the destination (drawing surface) viewport.
        Align the <min-y> of the source viewport with the smallest y value of the destination (drawing surface) viewport.
    */
    XMidYMin = AmanithSVG.SVGT_ASPECT_RATIO_ALIGN_XMIDYMIN,

    /*
        Force uniform scaling.
        Align the <max-x> of the source viewport with the maximum x value of the destination (drawing surface) viewport.
        Align the <min-y> of the source viewport with the smallest y value of the destination (drawing surface) viewport.
    */
    XMaxYMin = AmanithSVG.SVGT_ASPECT_RATIO_ALIGN_XMAXYMIN,

    /*
        Force uniform scaling.
        Align the <min-x> of the source viewport with the smallest x value of the destination (drawing surface) viewport.
        Align the <mid-y> of the source viewport with the midpoint y value of the destination (drawing surface) viewport.
    */
    XMinYMid = AmanithSVG.SVGT_ASPECT_RATIO_ALIGN_XMINYMID,

    /*
        Force uniform scaling.
        Align the <mid-x> of the source viewport with the midpoint x value of the destination (drawing surface) viewport.
        Align the <mid-y> of the source viewport with the midpoint y value of the destination (drawing surface) viewport.
    */
    XMidYMid = AmanithSVG.SVGT_ASPECT_RATIO_ALIGN_XMIDYMID,

    /*
        Force uniform scaling.
        Align the <max-x> of the source viewport with the maximum x value of the destination (drawing surface) viewport.
        Align the <mid-y> of the source viewport with the midpoint y value of the destination (drawing surface) viewport.
    */
    XMaxYMid = AmanithSVG.SVGT_ASPECT_RATIO_ALIGN_XMAXYMID,

    /*
        Force uniform scaling.
        Align the <min-x> of the source viewport with the smallest x value of the destination (drawing surface) viewport.
        Align the <max-y> of the source viewport with the maximum y value of the destination (drawing surface) viewport.
    */
    XMinYMax = AmanithSVG.SVGT_ASPECT_RATIO_ALIGN_XMINYMAX,

    /*
        Force uniform scaling.
        Align the <mid-x> of the source viewport with the midpoint x value of the destination (drawing surface) viewport.
        Align the <max-y> of the source viewport with the maximum y value of the destination (drawing surface) viewport.
    */
    XMidYMax = AmanithSVG.SVGT_ASPECT_RATIO_ALIGN_XMIDYMAX,

    /*
        Force uniform scaling.
        Align the <max-x> of the source viewport with the maximum x value of the destination (drawing surface) viewport.
        Align the <max-y> of the source viewport with the maximum y value of the destination (drawing surface) viewport.
    */
    XMaxYMax = AmanithSVG.SVGT_ASPECT_RATIO_ALIGN_XMAXYMAX
}

public enum SVGMeetOrSlice
{
    /*
        Scale the graphic such that:
        - aspect ratio is preserved
        - the entire viewBox is visible within the viewport
        - the viewBox is scaled up as much as possible, while still meeting the other criteria

        In this case, if the aspect ratio of the graphic does not match the viewport, some of the viewport will
        extend beyond the bounds of the viewBox (i.e., the area into which the viewBox will draw will be smaller
        than the viewport).
    */
    Meet = AmanithSVG.SVGT_ASPECT_RATIO_MEET,

    /*
        Scale the graphic such that:
        - aspect ratio is preserved
        - the entire viewport is covered by the viewBox
        - the viewBox is scaled down as much as possible, while still meeting the other criteria
        
        In this case, if the aspect ratio of the viewBox does not match the viewport, some of the viewBox will
        extend beyond the bounds of the viewport (i.e., the area into which the viewBox will draw is larger
        than the viewport).
    */
    Slice = AmanithSVG.SVGT_ASPECT_RATIO_SLICE
}

public enum SVGRenderingQuality
{
    /* Disables antialiasing */
    NonAntialiased = AmanithSVG.SVGT_RENDERING_QUALITY_NONANTIALIASED,
    /* Causes rendering to be done at the highest available speed */
    Faster = AmanithSVG.SVGT_RENDERING_QUALITY_FASTER,
    /* Causes rendering to be done with the highest available quality */
    Better = AmanithSVG.SVGT_RENDERING_QUALITY_BETTER
}

public class SVGAspectRatio
{
    // Constructor.
    public SVGAspectRatio()
    {
        this._alignment = SVGAlign.XMidYMid;
        this._meetOrSlice = SVGMeetOrSlice.Meet;
        this._changed = true;
    }

    // Set constructor.
    public SVGAspectRatio(SVGAlign alignment, SVGMeetOrSlice meetOrSlice)
    {
        this._alignment = alignment;
        this._meetOrSlice = meetOrSlice;
        this._changed = true;
    }

    // Alignment.
    public SVGAlign Alignment
    {
        get
        {
            return this._alignment;
        }
        set
        {
            this._alignment = value;
            this._changed = true;
        }
    }

    // Meet or slice.
    public SVGMeetOrSlice MeetOrSlice
    {
        get
        {
            return this._meetOrSlice;
        }
        set
        {
            this._meetOrSlice = value;
            this._changed = true;
        }
    }

    internal bool Changed
    {
        get
        {
            return this._changed;
        }
        set
        {
            this._changed = value;
        }
    }

    // Alignment.
    private SVGAlign _alignment;
    // Meet or slice.
    private SVGMeetOrSlice _meetOrSlice;
    // Keep track if some parameter has been changed.
    private bool _changed;
}

/*
    SVG document.

    An SVG document can be created through SVGAssets.CreateDocument function, specifying the xml text.
    The document will be parsed immediately, and the internal drawing tree will be created.

    Once the document has been created, it can be drawn several times onto one (or more) drawing surface.
    In order to draw a document:

    (1) create a drawing surface using SVGAssets.CreateSurface
    (2) call surface Draw method, specifying the document to draw
*/
public class SVGDocument : IDisposable
{
    // Constructor.
    internal SVGDocument(uint handle)
    {
        int err;
        float[] viewport = new float[4];
        uint[] aspectRatio = new uint[2];

        // keep track of the AmanithSVG document handle
        this._handle = handle;
        this._disposed = false;

        // get document viewport
        if ((err = AmanithSVG.svgtDocViewportGet(this._handle, viewport)) == AmanithSVG.SVGT_NO_ERROR)
        {
            this._viewport = new SVGViewport(viewport[0], viewport[1], viewport[2], viewport[3]);
        }
        else
        {
            this._viewport = null;
            // log an error message
            AmanithSVG.svgtErrorLog("Error getting document viewport: ", err);
        }

        // get viewport aspect ratio/alignment
        if ((err = AmanithSVG.svgtDocViewportAlignmentGet(this._handle, aspectRatio)) == AmanithSVG.SVGT_NO_ERROR)
        {
            this._aspectRatio = new SVGAspectRatio((SVGAlign)aspectRatio[0], (SVGMeetOrSlice)aspectRatio[1]);
        }
        else
        {
            this._aspectRatio = null;
            // log an error message
            AmanithSVG.svgtErrorLog("Error getting document aspect ratio/alignment: ", err);
        }
    }

    // Destructor.
    ~SVGDocument()
    {
        Dispose(false);
    }

    // Implement IDisposable.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // check to see if Dispose has already been called
        if (!this._disposed)
        {
            // if disposing equals true, dispose all managed and unmanaged resources
            if (disposing)
            {
                // dispose managed resources (nothing to do here)
            }
            // dispose unmanaged resources
            if (this._handle != AmanithSVG.SVGT_INVALID_HANDLE)
            {
                AmanithSVG.svgtDocDestroy(this._handle);
                this._handle = AmanithSVG.SVGT_INVALID_HANDLE;
            }
            // disposing has been done
            this._disposed = true;
        }
    }

    // If needed, update document viewport at AmanithSVG backend side; it returns true if the operation was completed successfully, else false.
    internal bool UpdateViewport()
    {
        int err;

        // set document viewport (AmanithSVG backend)
        if (this._viewport != null && this._viewport.Changed)
        {
            float[] viewport = new float[4] { this._viewport.X, this._viewport.Y, this._viewport.Width, this._viewport.Height };
            if ((err = AmanithSVG.svgtDocViewportSet(this._handle, viewport)) != AmanithSVG.SVGT_NO_ERROR)
            {
                // log an error message
                AmanithSVG.svgtErrorLog("Error setting surface viewport: ", err);
                return false;
            }
            this._viewport.Changed = false;
        }

        // set document viewport aspect ratio/alignment (AmanithSVG backend)
        if (this._aspectRatio != null && this._aspectRatio.Changed)
        {
            uint[] aspectRatio = new uint[2] { (uint)this._aspectRatio.Alignment, (uint)this._aspectRatio.MeetOrSlice };
            if ((err = AmanithSVG.svgtDocViewportAlignmentSet(this._handle, aspectRatio)) != AmanithSVG.SVGT_NO_ERROR)
            {
                // log an error message
                AmanithSVG.svgtErrorLog("Error setting document aspect ratio/alignment: ", err);
                return false;
            }
            this._aspectRatio.Changed = false;
        }

        return true;
    }

    /*
        Map a point, expressed in the document viewport system, into the surface viewport.
        The transformation will be performed according to the current document viewport and the
        current surface viewport.
    */
    public SVGPoint PointMap(SVGSurface surface, SVGPoint p)
    {
        SVGPoint zero = new SVGPoint(0.0f, 0.0f);

        if (surface != null && p != null)
        {
            int err;
            float[] dst = new float[2];

            // update document viewport (AmanithSVG backend)
            if (!this.UpdateViewport())
            {
                return zero;
            }
            // update surface viewport (AmanithSVG backend)
            if (!surface.UpdateViewport())
            {
                return zero;
            }
            // map the specified point
            if ((err = AmanithSVG.svgtPointMap(this._handle, surface.Handle, p.X, p.Y, dst)) != AmanithSVG.SVGT_NO_ERROR)
            {
                // log an error message
                AmanithSVG.svgtErrorLog("Error mapping a point: ", err);
                return zero;
            }
            // return the result
            return new SVGPoint(dst[0], dst[1]);
        }
        else
        {
            return zero;
        }
    }


    // AmanithSVG document handle (read only).
    public uint Handle
    {
        get
        {
            return this._handle;
        }
    }

    /*
        SVG content itself optionally can provide information about the appropriate viewport region for
        the content via the 'width' and 'height' XML attributes on the outermost <svg> element.
        Use this property to get the suggested viewport width, in pixels.

        It returns -1 (i.e. an invalid width) in the following cases:
        - the library has not previously been initialized through the svgtInit function
        - outermost element is not an <svg> element
        - outermost <svg> element doesn't have a 'width' attribute specified
        - outermost <svg> element has a 'width' attribute specified in relative measure units (i.e. em, ex, % percentage)
    */
    public float Width
    {
        get
        {
            return AmanithSVG.svgtDocWidth(this._handle);
        }
    }

    /*
        SVG content itself optionally can provide information about the appropriate viewport region for
        the content via the 'width' and 'height' XML attributes on the outermost <svg> element.
        Use this property to get the suggested viewport height, in pixels.

        It returns -1 (i.e. an invalid height) in the following cases:
        - the library has not previously been initialized through the svgtInit function
        - outermost element is not an <svg> element
        - outermost <svg> element doesn't have a 'height' attribute specified
        - outermost <svg> element has a 'height' attribute specified in relative measure units (i.e. em, ex, % percentage)
    */
    public float Height
    {
        get
        {
            return AmanithSVG.svgtDocHeight(this._handle);
        }
    }

    /*
        The document (logical) viewport to map onto the destination (drawing surface) viewport.
        When an SVG document has been created through the SVGAssets.CreateDocument function, the initial
        value of its viewport is equal to the 'viewBox' attribute present in the outermost <svg> element.
    */
    public SVGViewport Viewport
    {
        get
        {
            return this._viewport;
        }
        set
        {
            if (value != null)
            {
                this._viewport = value;
            }
        }
    }

    /*
        Viewport aspect ratio.
        The alignment parameter indicates whether to force uniform scaling and, if so, the alignment method to use in case
        the aspect ratio of the document viewport doesn't match the aspect ratio of the surface viewport.
    */
    public SVGAspectRatio AspectRatio
    {
        get
        {
            return this._aspectRatio;
        }
        set
        {
            if (value != null)
            {
                this._aspectRatio = value;
            }
        }
    }

    // Document handle.
    private uint _handle;
    // Track whether Dispose has been called.
    private bool _disposed;
    // Viewport.
    private SVGViewport _viewport;
    // Viewport aspect ratio/alignment.
    private SVGAspectRatio _aspectRatio;
}

/*
    Drawing surface.

    A drawing surface is just a rectangular area made of pixels, where each pixel is represented internally by a 32bit unsigned integer.
    A pixel is made of four 8-bit components: red, green, blue, alpha.
 
    Coordinate system is the same of SVG specifications: top/left pixel has coordinate (0, 0), with the positive x-axis pointing towards
    the right and the positive y-axis pointing down.
*/
public class SVGSurface : IDisposable
{
    // Constructor.
    internal SVGSurface(uint handle)
    {
        int err;
        float[] viewport = new float[4];

        // keep track of the AmanithSVG surface handle
        this._handle = handle;
        this._disposed = false;

        // get surface viewport
        if ((err = AmanithSVG.svgtSurfaceViewportGet(this._handle, viewport)) == AmanithSVG.SVGT_NO_ERROR)
            this._viewport = new SVGViewport(viewport[0], viewport[1], viewport[2], viewport[3]);
        else
        {
            this._viewport = null;
            // log an error message
            AmanithSVG.svgtErrorLog("Error getting surface viewport: ", err);
        }
    }

    // Destructor.
    ~SVGSurface()
    {
        Dispose(false);
    }

    // Implement IDisposable.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // check to see if Dispose has already been called
        if (!this._disposed)
        {
            // if disposing equals true, dispose all managed and unmanaged resources
            if (disposing)
            {
                // dispose managed resources (nothing to do here)
            }
            // dispose unmanaged resources
            if (this._handle != AmanithSVG.SVGT_INVALID_HANDLE)
            {
                AmanithSVG.svgtSurfaceDestroy(this._handle);
                this._handle = AmanithSVG.SVGT_INVALID_HANDLE;
            }
            // disposing has been done
            this._disposed = true;
        }
    }

    /*
        Resize the surface, specifying new dimensions in pixels; it returns true if the operation was completed successfully, else false.

        After resizing, the surface viewport will be reset to the whole surface.
    */
    public bool Resize(uint newWidth, uint newHeight)
    {
        int err;

        if ((err = AmanithSVG.svgtSurfaceResize(this._handle, newWidth, newHeight)) != AmanithSVG.SVGT_NO_ERROR)
        {
            AmanithSVG.svgtErrorLog("Surface resize error: ", err);
            return false;
        }
        // svgtSurfaceResize will reset the surface viewport, so we must perform the same operation here
        this._viewport = new SVGViewport(0.0f, 0.0f, (float)this.Width, (float)this.Height);
        return true;
    }

    // If needed, update surface viewport at AmanithSVG backend side; it returns true if the operation was completed successfully, else false.
    internal bool UpdateViewport()
    {
        int err;
        // set surface viewport (AmanithSVG backend)
        if (this._viewport != null && this._viewport.Changed)
        {
            float[] viewport = new float[4] { this._viewport.X, this._viewport.Y, this._viewport.Width, this._viewport.Height };
            if ((err = AmanithSVG.svgtSurfaceViewportSet(this._handle, viewport)) != AmanithSVG.SVGT_NO_ERROR)
            {
                // log an error message
                AmanithSVG.svgtErrorLog("Error setting surface viewport: ", err);
                return false;
            }
            this._viewport.Changed = false;
        }

        return true;
    }

    /*
        Draw an SVG document, on this drawing surface.

        First the drawing surface is cleared if a valid (i.e. not null) clear color is provided.
        Then the specified document, if valid, is drawn.

        It returns true if the operation was completed successfully, else false.
    */
    public bool Draw(SVGDocument document, SVGColor clearColor, SVGRenderingQuality renderingQuality)
    {
        int err;

        // set clear color
        if (!this.SetClearColor(clearColor))
        {
            return false;
        }

        if (document != null)
        {
            // update document viewport (AmanithSVG backend)
            if (!document.UpdateViewport())
            {
                return false;
            }
            // update surface viewport (AmanithSVG backend)
            if (!this.UpdateViewport())
            {
                return false;
            }

            // draw the document
            if ((err = AmanithSVG.svgtDocDraw(document.Handle, this.Handle, (uint)renderingQuality)) != AmanithSVG.SVGT_NO_ERROR)
            {
                AmanithSVG.svgtErrorLog("Surface draw error (drawing document): ", err);
                return false;
            }
        }
        return true;
    }

    public bool Draw(SVGPackedBin bin, SVGColor clearColor, SVGRenderingQuality renderingQuality)
    {
        int err;

        // set clear color
        if (!this.SetClearColor(clearColor))
        {
            return false;
        }

        if (bin != null && bin.Rectangles.Length > 0)
        {
            //if ((err = AmanithSVG.svgtPackingDraw(bin.Index, 0, (uint)bin.Rectangles.Length, this.Handle, (uint)renderingQuality)) != AmanithSVG.SVGT_NO_ERROR)
            if ((err = AmanithSVG.svgtPackingRectsDraw(bin.NativeRectangles, (uint)bin.Rectangles.Length, this.Handle, (uint)renderingQuality)) != AmanithSVG.SVGT_NO_ERROR)
            {
                AmanithSVG.svgtErrorLog("Surface draw error (drawing packed bin): ", err);
                return false;
            }
        }
        return true;
    }

    /*
        Copy drawing surface content (i.e. pixels) into the specified destination array.

        If the 'redBlueSwap' flag is true, the copy process will also swap red and blue channels for each pixel; this
        kind of swap could be useful when dealing with OpenGL/Direct3D texture uploads (RGBA or BGRA formats).

        If the 'dilateEdgesFix' flag is true, the copy process will also perform a 1-pixel dilate post-filter; this
        dilate filter could be useful when surface pixels will be uploaded to OpenGL/Direct3D bilinear-filtered textures.
    */
    public bool Copy(int[] dstPixels32, bool redBlueSwap, bool dilateEdgesFix)
    {
        if (dstPixels32 == null)
        {
            return false;
        }
        else
        {
            int n = (int)Width * (int)Height;
            // dstPixels32 must contain at least 'n' pixels
            if (dstPixels32.Length < n)
            {
                return false;
            }
            else
            {
                bool ok = false;
                // "pin" the array in memory, so we can pass direct pointer to the native plugin, without costly marshaling array of structures
                GCHandle bufferHandle = GCHandle.Alloc(dstPixels32, GCHandleType.Pinned);

                try
                {
                    System.IntPtr bufferPtr = bufferHandle.AddrOfPinnedObject();
                    // copy pixels from internal drawing surface to destination pixels array; NB: AmanithSVG buffer is always in BGRA format (i.e. B = LSB, A = MSB)
                    if (AmanithSVG.svgtSurfaceCopy(this.Handle, bufferPtr,
                                                   redBlueSwap ? AmanithSVG.SVGT_TRUE : AmanithSVG.SVGT_FALSE,
                                                   dilateEdgesFix ? AmanithSVG.SVGT_TRUE : AmanithSVG.SVGT_FALSE) == AmanithSVG.SVGT_NO_ERROR)
                    {
                        ok = true;
                    }
                }
                finally
                {
                    // free the pinned array handle
                    if (bufferHandle.IsAllocated)
                    {
                        bufferHandle.Free();
                    }
                }

                return ok;
            }
        }
    }

#if UNITY_ENGINE
    private bool Copy(Texture2D texture, bool dilateEdgesFix)
    {
        bool ok = false;
        uint pixelsCount = this.Width * this.Height;
        bool redBlueSwap = texture.format == TextureFormat.RGBA32;

        if (redBlueSwap || dilateEdgesFix)
        {
            /*
            // an alternative way to do the same thing

            int[] tempBuffer = new int[pixelsCount];
            // "pin" the array in memory, so we can pass direct pointer to the native plugin, without costly marshaling array of structures
            GCHandle tempBufferHandle = GCHandle.Alloc(tempBuffer, GCHandleType.Pinned);

            try
            {
                System.IntPtr tempBufferPtr = tempBufferHandle.AddrOfPinnedObject();
                // copy pixels from internal drawing surface to destination pixels array; NB: AmanithSVG buffer is always in BGRA format (i.e. B = LSB, A = MSB)
                if (AmanithSVG.svgtSurfaceCopy(this.Handle, tempBufferPtr,
                                               redBlueSwap ? AmanithSVG.SVGT_TRUE : AmanithSVG.SVGT_FALSE,
                                               dilateEdgesFix ? AmanithSVG.SVGT_TRUE : AmanithSVG.SVGT_FALSE) == AmanithSVG.SVGT_NO_ERROR) {
                    // fill texture pixel memory with raw data; NB: later we must call texture.Apply to actually upload it to the GPU
                    texture.LoadRawTextureData(tempBufferPtr, (int)pixelsCount * 4);
                    ok = true;
                }
            }
            finally
            {
                // free the pinned array handle
                if (tempBufferHandle.IsAllocated)
                {
                    tempBufferHandle.Free();
                }
            }
            */

            System.IntPtr tempBufferPtr = System.IntPtr.Zero;

            try
            {
                tempBufferPtr = Marshal.AllocHGlobal((int)pixelsCount * 4);
                // copy pixels from internal drawing surface to destination pixels array; NB: AmanithSVG buffer is always in BGRA format (i.e. B = LSB, A = MSB)
                if (AmanithSVG.svgtSurfaceCopy(this.Handle, tempBufferPtr,
                                               redBlueSwap ? AmanithSVG.SVGT_TRUE : AmanithSVG.SVGT_FALSE,
                                               dilateEdgesFix ? AmanithSVG.SVGT_TRUE : AmanithSVG.SVGT_FALSE) == AmanithSVG.SVGT_NO_ERROR)
                {
                    // fill texture pixel memory with raw data; NB: later we must call texture.Apply to actually upload it to the GPU
                    texture.LoadRawTextureData(tempBufferPtr, (int)pixelsCount * 4);
                    ok = true;
                }
            }
            finally
            {
                if (tempBufferPtr != System.IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(tempBufferPtr);
                }
            }
        }
        else
        {
            // we can use AmanithSVG pixels directly; NB: AmanithSVG buffer is always in BGRA format (i.e. B = LSB, A = MSB)
            System.IntPtr pixels = AmanithSVG.svgtSurfacePixels(this.Handle);
            // fill texture pixel memory with raw data; NB: later we must call texture.Apply to actually upload it to the GPU
            texture.LoadRawTextureData(pixels, (int)pixelsCount * 4);
            ok = true;
        }

        return ok;
    }

    /*
        Create a 2D texture compatible with the drawing surface.
        NB: textures passed to Copy and CopyAndDestroy must be created through this function.
    */
    public Texture2D CreateCompatibleTexture(bool bilinearFilter, bool wrapRepeat)
    {
        uint width = this.Width;
        uint height = this.Height;
        // try to use BGRA format, because on little endian architectures will speedup the upload of texture pixels
        // by avoiding swizzling (e.g. glTexImage2D / glTexSubImage2D)
        bool bgraSupport = SystemInfo.SupportsTextureFormat(TextureFormat.BGRA32);
        TextureFormat format = bgraSupport ? TextureFormat.BGRA32 : TextureFormat.RGBA32;

        Texture2D texture = new Texture2D((int)width, (int)height, format, false);

        if (texture != null)
        {
            texture.filterMode = bilinearFilter ? FilterMode.Bilinear : FilterMode.Point;
            if ((SystemInfo.npotSupport == NPOTSupport.Full) || ((SVGUtils.IsPow2(width)) && (SVGUtils.IsPow2(height))))
            {
                texture.wrapMode = wrapRepeat ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
            }
            else
            {
                texture.wrapMode = TextureWrapMode.Clamp;
            }
            texture.anisoLevel = 1;
        #if UNITY_EDITOR
            texture.alphaIsTransparency = true;
        #endif
        }
        return texture;
    }

    /*
        Copy drawing surface content into the specified texture.
        This function is useful for managed environments (e.g. C#, Unity, Java, Android), where the use of a direct pixels
        access is not advisable nor comfortable.
        It returns true if the operation was completed successfully, else false.
    */
    public bool Copy(Texture2D texture)
    {
        if (texture != null)
        {
            return this.Copy(texture, (texture.filterMode != FilterMode.Point) ? true : false);
        }
        else
        {
            AmanithSVG.svgtErrorLog("Surface copy error, specified texture is null: ", AmanithSVG.SVGT_ILLEGAL_ARGUMENT_ERROR);
            return false;
        }
    }

    public bool CopyAndDestroy(Texture2D texture)
    {
        if (texture != null)
        {
            // set the target texture handle
            System.IntPtr hwPtr = texture.GetNativeTexturePtr();
            int err = AmanithSVG.svgtSurfaceTexturePtrSet(this.Handle, hwPtr,
                                                          (uint)texture.width, (uint)texture.height,
                                                          (texture.format == TextureFormat.BGRA32) ? AmanithSVG.SVGT_TRUE : AmanithSVG.SVGT_FALSE, 
                                                          (texture.filterMode != FilterMode.Point) ? AmanithSVG.SVGT_TRUE : AmanithSVG.SVGT_FALSE);

            // check for errors
            if (err != AmanithSVG.SVGT_NO_ERROR)
            {
                AmanithSVG.svgtErrorLog("Surface copy (to texture) and destroy error: ", err);
                return false;
            }

            GL.IssuePluginEvent(AmanithSVG.svgtSurfaceTextureCopyAndDestroyFuncGet(), (int)this.Handle);
            // set the surface internal handle to invalid, because the copy&destroy will take care to free the surface memory after the copy
            // NB: after the copy this surface won't be usable anymore
            this._handle = AmanithSVG.SVGT_INVALID_HANDLE;
            return true;
        }
        else
        {
            AmanithSVG.svgtErrorLog("Surface copy and destroy error, specified texture is null: ", AmanithSVG.SVGT_ILLEGAL_ARGUMENT_ERROR);
            return false;
        }
    }
#endif // UNITY_ENGINE

    // AmanithSVG surface handle (read only).
    public uint Handle
    {
        get
        {
            return this._handle;
        }
    }

    // Get current surface width, in pixels.
    public uint Width
    {
        get
        {
            return AmanithSVG.svgtSurfaceWidth(this._handle);
        }
    }

    // Get current surface height, in pixels.
    public uint Height
    {
        get
        {
            return AmanithSVG.svgtSurfaceHeight(this._handle);
        }
    }

    /*
        The surface viewport (i.e. a drawing surface rectangular area), where to map the source document viewport.
        The combined use of surface and document viewport, induces a transformation matrix, that will be used to draw
        the whole SVG document. The induced matrix grants that the document viewport is mapped onto the surface
        viewport (respecting the specified alignment): all SVG content will be drawn accordingly.
    */
    public SVGViewport Viewport
    {
        get
        {
            return this._viewport;
        }
        set
        {
            if (value != null)
            {
                this._viewport = value;
            }
        }
    }

    // The maximum width/height dimension that can be specified to the SVGSurface.Resize and SVGAssets.CreateSurface functions.
    public static uint MaxDimension
    {
        get
        {
            return AmanithSVG.svgtSurfaceMaxDimension();
        }
    }

    private bool SetClearColor(SVGColor clearColor)
    {
        int err;

        if (clearColor != null)
        {
            // clear the whole surface, with the specified color
            if ((err = AmanithSVG.svgtClearColor(clearColor.Red, clearColor.Green, clearColor.Blue, clearColor.Alpha)) != AmanithSVG.SVGT_NO_ERROR)
            {
                AmanithSVG.svgtErrorLog("Surface draw error (setting clear color): ", err);
                return false;
            }
            if ((err = AmanithSVG.svgtClearPerform(AmanithSVG.SVGT_TRUE)) != AmanithSVG.SVGT_NO_ERROR)
            {
                AmanithSVG.svgtErrorLog("Surface draw error (enabling clear): ", err);
                return false;
            }
            return true;
        }
        else
        {
            // do not clear the surface
            if ((err = AmanithSVG.svgtClearPerform(AmanithSVG.SVGT_FALSE)) != AmanithSVG.SVGT_NO_ERROR)
            {
                AmanithSVG.svgtErrorLog("Surface draw error (disabling clear): ", err);
                return false;
            }
            return true;
        }
    }

    // Surface handle.
    private uint _handle = AmanithSVG.SVGT_INVALID_HANDLE;
    // Track whether Dispose has been called.
    private bool _disposed = false;
    // Viewport.
    private SVGViewport _viewport;
}

public enum SVGScalerMatchMode
{
    // Do not scale packed SVG.
    None = 0,
    // Scale each packed SVG according to width.
    Horizontal = 1,
    // Scale each packed SVG according to height.
    Vertical = 2,
    // Scale each packed SVG according to the minimum dimension between width and height.
    MinDimension = 3,
    // Scale each packed SVG according to the maximum dimension between width and height.
    MaxDimension = 4,
    // Expand the canvas area either horizontally or vertically, so the size of the canvas will never be smaller than the reference.
    Expand = 5,
    // Crop the canvas area either horizontally or vertically, so the size of the canvas will never be larger than the reference.
    Shrink = 6,
    // Scale each packed SVG with the width as reference, the height as reference, or something in between.
    MatchWidthOrHeight = 7
};

public class SVGScaler
{
    // Constructor.
    public SVGScaler(float referenceWidth, float referenceHeight, SVGScalerMatchMode matchMode, float match, float offsetScale)
    {
        _referenceWidth = referenceWidth;
        _referenceHeight = referenceHeight;
        _matchMode = matchMode;
        _match = match;
        _offsetScale = offsetScale;
    }

    public float ReferenceWidth
    {
        get
        {
            return this._referenceWidth;
        }
        set
        {
            this._referenceWidth = value;
        }
    }

    public float ReferenceHeight
    {
        get
        {
            return this._referenceHeight;
        }
        set
        {
            this._referenceHeight = value;
        }
    }

    public SVGScalerMatchMode MatchMode
    {
        get
        {
            return this._matchMode;
        }
        set
        {
            this._matchMode = value;
        }
    }

    public float Match
    {
        get
        {
            return this._match;
        }
        set
        {
            this._match = value;
        }
    }

    public float OffsetScale
    {
        get
        {
            return this._offsetScale;
        }
        set
        {
            this._offsetScale = value;
        }
    }

    public float ScaleFactorCalc(float currentWidth, float currentHeight)
    {
        float scale;
        bool referenceLandscape, currentLandscape;

        switch (this._matchMode)
        {
            case SVGScalerMatchMode.Horizontal:
                scale = currentWidth / this._referenceWidth;
                break;

            case SVGScalerMatchMode.Vertical:
                scale = currentHeight / this._referenceHeight;
                break;

            case SVGScalerMatchMode.MinDimension:
                referenceLandscape = (this._referenceWidth > this._referenceHeight) ? true : false;
                currentLandscape = (currentWidth > currentHeight) ? true : false;
                if (referenceLandscape != currentLandscape)
                {
                    scale = (currentWidth <= currentHeight) ? (currentWidth / this._referenceHeight) : (currentHeight / this._referenceWidth);
                }
                else
                {
                    scale = (currentWidth <= currentHeight) ? (currentWidth / this._referenceWidth) : (currentHeight / this._referenceHeight);
                }
                break;

            case SVGScalerMatchMode.MaxDimension:
                referenceLandscape = (this._referenceWidth > this._referenceHeight) ? true : false;
                currentLandscape = (currentWidth > currentHeight) ? true : false;
                if (referenceLandscape != currentLandscape)
                {
                    scale = (currentWidth >= currentHeight) ? (currentWidth / this._referenceHeight) : (currentHeight / this._referenceWidth);
                }
                else
                {
                    scale = (currentWidth >= currentHeight) ? (currentWidth / this._referenceWidth) : (currentHeight / this._referenceHeight);
                }
                break;

            case SVGScalerMatchMode.Expand:
                scale = Math.Max(currentWidth / this._referenceWidth, currentHeight / this._referenceHeight);
                break;

            case SVGScalerMatchMode.Shrink:
                scale = Math.Min(currentWidth / this._referenceWidth, currentHeight / this._referenceHeight);
                break;

            case SVGScalerMatchMode.MatchWidthOrHeight:
            {
                /*
                    We take the log of the relative width and height before taking the average. Then we transform it back in the original space.
                    The reason to transform in and out of logarithmic space is to have better behavior.
                    If one axis has twice resolution and the other has half, it should even out if widthOrHeight value is at 0.5.
                    In normal space the average would be (0.5 + 2) / 2 = 1.25
                    In logarithmic space the average is (-1 + 1) / 2 = 0
                */
                float logWidth = (float)(Math.Log(currentWidth / _referenceWidth) / Math.Log(2));
                float logHeight = (float)(Math.Log(currentHeight / _referenceHeight) / Math.Log(2));
                // clamp between 0 and 1
                float t = Math.Max(0, Math.Min(1.0f, this._match));
                // lerp
                float logWeightedAverage = ((1.0f - t) * logWidth) + (t * logHeight);
                scale = (float)Math.Pow(2, logWeightedAverage);
                break;
            }

            default:
                scale = 1.0f;
                break;
        }

        return (scale * this._offsetScale);
    }

    private float _referenceWidth;
    private float _referenceHeight;
    private SVGScalerMatchMode _matchMode;
    private float _match;
    private float _offsetScale;
}

public class SVGPackedRectangle
{
    // Constructor.
    public SVGPackedRectangle(uint docHandle, uint elemIdx, string name, int originalX, int originalY, int x, int y, int width, int height, int zOrder)
    {
        this._docHandle = docHandle;
        this._elemIdx = elemIdx;
        this._name = name;
        this._originalX = originalX;
        this._originalY = originalY;
        this._x = x;
        this._y = y;
        this._width = width;
        this._height = height;
        this._zOrder = zOrder;
    }

    public uint DocHandle
    {
        get
        {
            return this._docHandle;
        }
    }

    public uint ElemIdx
    {
        get
        {
            return this._elemIdx;
        }
    }

    public string Name
    {
        get
        {
            return this._name;
        }
    }

    public int OriginalX
    {
        get
        {
            return this._originalX;
        }
    }
    
    public int OriginalY
    {
        get
        {
            return this._originalY;
        }
    }

    public int X
    {
        get
        {
            return this._x;
        }
    }

    public int Y
    {
        get
        {
            return this._y;
        }
    }

    public int Width
    {
        get
        {
            return this._width;
        }
    }

    public int Height
    {
        get
        {
            return this._height;
        }
    }

    public int ZOrder
    {
        get
        {
            return this._zOrder;
        }
    }

    // SVG document handle
    private uint _docHandle;
    // SVG element (unique) identifier inside its document
    private uint _elemIdx;
    // Generated name
    private string _name;
    // Original top/left corner isnide the drawing surface
    private int _originalX;
    private int _originalY;
    // Top/left corner
    private int _x;
    private int _y;
    // Dimensions in pixels
    private int _width;
    private int _height;
    // Z-order
    private int _zOrder;
}

public class SVGPackedBin
{
    // Constructor
    internal SVGPackedBin(uint index, uint width, uint height, uint rectsCount, System.IntPtr nativeRectsPtr)
    {
        this._index = index;
        this._width = width;
        this._height = height;
        this._rects = new SVGPackedRectangle[rectsCount];
        this._nativeRects = System.IntPtr.Zero;
        this.Build(nativeRectsPtr, rectsCount);
    }

    // Destructor.
    ~SVGPackedBin()
    {
        if (this._nativeRects != System.IntPtr.Zero)
        {
            Marshal.FreeHGlobal(this._nativeRects);
        }
    }

    // Bin index
    public uint Index
    {
        get
        {
            return this._index;
        }
    }
    
    // Bin width, in pixels
    public uint Width
    {
        get
        {
            return this._width;
        }
    }
    
    // Bin height, in pixels
    public uint Height
    {
        get
        {
            return this._height;
        }
    }
    
    // Packed rectangles inside the bin
    internal SVGPackedRectangle[] Rectangles
    {
        get
        {
            return this._rects;
        }
    }

    internal System.IntPtr NativeRectangles
    {
        get
        {
            return this._nativeRects;
        }
    }

    private string GenElementName(AmanithSVG.SVGTPackedRect rect)
    {
        // build element name to be displayed in Unity editor
        return (rect.elemName != System.IntPtr.Zero) ? System.Runtime.InteropServices.Marshal.PtrToStringAnsi(rect.elemName) : rect.elemIdx.ToString();
    }

    private void Build(System.IntPtr nativeRectsPtr, uint rectsCount)
    {
        uint i;
    #if UNITY_WP_8_1 && !UNITY_EDITOR
        int rectSize = Marshal.SizeOf<AmanithSVG.SVGTPackedRect>();
    #else
        int rectSize = Marshal.SizeOf(typeof(AmanithSVG.SVGTPackedRect));
    #endif
        System.IntPtr rectsCopyPtr = Marshal.AllocHGlobal((int)rectsCount * rectSize);

        // keep track of the native buffer
        this._nativeRects = rectsCopyPtr;

        // fill rectangles
        for (i = 0; i < rectsCount; ++i)
        {
            // rectangle generated by AmanithSVG packer
        #if UNITY_WP_8_1 && !UNITY_EDITOR
            AmanithSVG.SVGTPackedRect rect = (AmanithSVG.SVGTPackedRect)Marshal.PtrToStructure<AmanithSVG.SVGTPackedRect>(rectsPtr);
        #else
            AmanithSVG.SVGTPackedRect rect = (AmanithSVG.SVGTPackedRect)Marshal.PtrToStructure(nativeRectsPtr, typeof(AmanithSVG.SVGTPackedRect));
        #endif
            // set the rectangle
            this._rects[i] = new SVGPackedRectangle(rect.docHandle, rect.elemIdx, this.GenElementName(rect), rect.originalX, rect.originalY, rect.x, rect.y, rect.width, rect.height, rect.zOrder/*, rect.dstViewportWidth, rect.dstViewportHeight*/);
            // copy 
            Marshal.StructureToPtr(rect, rectsCopyPtr, false);
            // next packed rectangle
            nativeRectsPtr = (System.IntPtr)(nativeRectsPtr.ToInt64() + rectSize);
            rectsCopyPtr = (System.IntPtr)(rectsCopyPtr.ToInt64() + rectSize);
        }
    }

    // Bin index.
    private uint _index;
    // Width in pixels.
    private uint _width;
    // Height in pixels.
    private uint _height;
    // Packed rectangles inside the bin.
    SVGPackedRectangle[] _rects;
    // A copy of (native) rectangles; to be used for drawing purposes only.
    System.IntPtr _nativeRects;
}

public class SVGPacker
{
    // Constructor.
    internal SVGPacker(float scale, uint maxTexturesDimension, uint border, bool pow2Textures)
    {
        this._scale = Math.Abs(scale);
        this._maxTexturesDimension = maxTexturesDimension;
        this._border = border;
        this._pow2Textures = pow2Textures;
        this.FixMaxDimension();
        this.FixBorder();
    }

    public float Scale
    {
        get
        {
            return this._scale;
        }
    }

    public uint MaxTexturesDimension
    {
        get
        {
            return this._maxTexturesDimension;
        }

        set
        {
            this._maxTexturesDimension = value;
            this.FixMaxDimension();
            this.FixBorder();
        }
    }
    
    public uint Border
    {
        get
        {
            return this._border;
        }

        set
        {
            this._border = value;
            this.FixBorder();
        }
    }

    public bool Pow2Textures
    {
        get
        {
            return this._pow2Textures;
        }

        set
        {
            this._pow2Textures = value;
            this.FixMaxDimension();
            this.FixBorder();
        }
    }

    /*!
        Start a packing task: one or more SVG documents will be collected and packed into bins, for the generation of atlases.

        Every collected SVG document/element will be packed into rectangular bins, whose dimensions won't exceed the specified 'maxTexturesDimension' (see constructor), in pixels.
        If true, 'pow2Textures' (see constructor) will force bins to have power-of-two dimensions.
        Each rectangle will be separated from the others by the specified 'border' (see constructor), in pixels.
        The specified 'scale' (see constructor) factor will be applied to all collected SVG documents/elements, in order to realize resolution-independent atlases.
    */
    public bool Begin()
    {
        int err = AmanithSVG.svgtPackingBegin(this._maxTexturesDimension, this._border, this._pow2Textures ? AmanithSVG.SVGT_TRUE : AmanithSVG.SVGT_FALSE, this._scale);
        // check for errors
        if (err != AmanithSVG.SVGT_NO_ERROR)
        {
            AmanithSVG.svgtErrorLog("SVGPacker.Begin error: ", err);
            return false;
        }
        return true;
    }

    /*!
        Add an SVG document to the current packing task.

        If true, 'explodeGroups' tells the packer to not pack the whole SVG document, but instead to pack each first-level element separately.
        The additional 'scale' is used to adjust the document content to the other documents involved in the current packing process.

        The function will return some useful information, an array of 2 entries and it will be filled with:
        - info[0] = number of collected bounding boxes
        - info[1] = the actual number of packed bounding boxes (boxes whose dimensions exceed the 'maxTexturesDimension' value specified through the constructor, will be discarded)
    */
    public uint[] Add(SVGDocument svgDoc, bool explodeGroup, float scale)
    {
        uint[] info = new uint[2];
        // add an SVG document to the current packing task, and get back information about collected bounding boxes
        int err = AmanithSVG.svgtPackingAdd(svgDoc.Handle, explodeGroup ? AmanithSVG.SVGT_TRUE : AmanithSVG.SVGT_FALSE, scale, info);
        
        // check for errors
        if (err != AmanithSVG.SVGT_NO_ERROR)
        {
            AmanithSVG.svgtErrorLog("SVGPacker.Add error: ", err);
            return null;
        }
        // info[0] = number of collected bounding boxes
        // info[1] = the actual number of packed bounding boxes (boxes whose dimensions exceed the 'maxDimension' value specified to the svgtPackingBegin function, will be discarded)
        return info;
    }

    /*!
        Close the current packing task and, if specified, perform the real packing algorithm.

        All collected SVG documents/elements (actually their bounding boxes) are packed into bins for later use (i.e. atlases generation).
        After calling this function, the application could use the SVGSurface.Draw method in order to draw the returned packed elements.
    */
    public SVGPackedBin[] End(bool performPacking)
    {
        int err, binsCount;
        uint i, j;
        uint[] binInfo;
        SVGPackedBin[] bins;
        
        // close the current packing task
        if ((err = AmanithSVG.svgtPackingEnd(performPacking ? AmanithSVG.SVGT_TRUE : AmanithSVG.SVGT_FALSE)) != AmanithSVG.SVGT_NO_ERROR)
        {
            AmanithSVG.svgtErrorLog("SVGPacker.End error: ", err);
            return null;
        }
        // if requested, close the packing process without doing anything
        if (!performPacking)
        {
            return null;
        }
        // get number of generated bins
        binsCount = AmanithSVG.svgtPackingBinsCount();
        if (binsCount <= 0)
        {
            return null;
        }
        // allocate space for bins
        bins = new SVGPackedBin[binsCount];
        if (bins == null)
        {
            return null;
        }
        // allocate space to store information of a single bin
        binInfo = new uint[3];
        if (binInfo == null)
        {
            return null;
        }

        // fill bins information
        j = (uint)binsCount;
        for (i = 0; i < j; ++i)
        {
            System.IntPtr rectsPtr;

            // get bin information
            if ((err = AmanithSVG.svgtPackingBinInfo(i, binInfo)) != AmanithSVG.SVGT_NO_ERROR)
            {
                AmanithSVG.svgtErrorLog("SVGPacker.End error: ", err);
                return null;
            }
            // get packed rectangles
            rectsPtr = AmanithSVG.svgtPackingBinRects(i);
            if (rectsPtr == System.IntPtr.Zero)
            {
                return null;
            }

            bins[i] = new SVGPackedBin(i, binInfo[0], binInfo[1], binInfo[2], rectsPtr);
        }
        return bins;
    }

    private void FixMaxDimension()
    {
        if (this._maxTexturesDimension == 0)
        {
            this._maxTexturesDimension = 1;
        }
        else
        {
            // check power-of-two option
            if (this._pow2Textures && (!SVGUtils.IsPow2(this._maxTexturesDimension)))
                // set maxTexturesDimension to the smallest power of two value greater (or equal) to it
                this._maxTexturesDimension = SVGUtils.Pow2Get(this._maxTexturesDimension);
        }
    }

    private void FixBorder()
    {
        // border must allow a packable region of at least one pixel
        uint maxAllowedBorder = ((this._maxTexturesDimension & 1) != 0) ? (this._maxTexturesDimension / 2) : ((this._maxTexturesDimension - 1) / 2);
        if (this._border > maxAllowedBorder)
        {
            this._border = maxAllowedBorder;
        }
    }

    private float _scale;
    private uint _maxTexturesDimension;
    private uint _border;
    private bool _pow2Textures;
}

static public class SVGAssets
{
    // Constructor.
    static SVGAssets()
    {
        SVGAssets.m_Initialized = false;
    }

    public static uint ScreenResolutionWidth
    {
        get
        {
        #if UNITY_ENGINE
            if (Application.isPlaying)
            {
                return (uint)Screen.width;
            }
            Vector2 view = SVGUtils.GetGameView();
            return (uint)view.x;
        #else
            // NB: you MUST implement this functionality according to the underlying graphics system
            return 1024;
        #endif
        }
    }
    
    public static uint ScreenResolutionHeight
    {
        get
        {
        #if UNITY_ENGINE
            if (Application.isPlaying)
            {
                return (uint)Screen.height;
            }
            Vector2 view = SVGUtils.GetGameView();
            return (uint)view.y;
        #else
            // NB: you MUST implement this functionality according to the underlying graphics system
            return 768;
        #endif
        }
    }

    public static float ScreenDpi
    {
        get
        {
        #if UNITY_ENGINE
            float dpi = Screen.dpi;
            return (dpi <= 0.0f ? 96.0f : dpi);
        #else
            // NB: you MUST implement this functionality according to the underlying graphics system
            return 96.0f;
        #endif
        }
    }

#if UNITY_ENGINE
    public static DeviceOrientation DeviceOrientation
    {
        get
        {
        #if UNITY_EDITOR
            return ((SVGAssets.ScreenResolutionHeight > SVGAssets.ScreenResolutionWidth) ? DeviceOrientation.Portrait : DeviceOrientation.LandscapeLeft);
        #else
            return Input.deviceOrientation;
        #endif
        }
    }
#endif

    // Create a drawing surface, specifying its dimensions in pixels.
    public static SVGSurface CreateSurface(uint width, uint height)
    {
        uint handle;

        // initialize the library, if required
        if (!SVGAssets.Init())
        {
            return null;
        }
        // create the surface
        if ((handle = AmanithSVG.svgtSurfaceCreate(width, height)) != AmanithSVG.SVGT_INVALID_HANDLE)
        {
            return new SVGSurface(handle);
        }
        AmanithSVG.svgtErrorLog("CreateSurface error (allocating surface): ", AmanithSVG.SVGT_OUT_OF_MEMORY_ERROR);
        return null;
    }

    // Create and load an SVG document, specifying the whole xml string.
    public static SVGDocument CreateDocument(string xmlText)
    {
        uint handle;

        // initialize the library, if required
        if (!SVGAssets.Init())
        {
            return null;
        }
        // create the document
        if ((handle = AmanithSVG.svgtDocCreate(xmlText)) != AmanithSVG.SVGT_INVALID_HANDLE)
        {
            return new SVGDocument(handle);
        }

        AmanithSVG.svgtErrorLog("CreateDocument error (parsing document): ", AmanithSVG.SVGT_OUT_OF_MEMORY_ERROR);
        return null;
    }

    public static SVGPacker CreatePacker(float scale, uint maxTexturesDimension, uint border, bool pow2Textures)
    {
        // initialize the library, if required
        if (!SVGAssets.Init())
        {
            return null;
        }
        
        return new SVGPacker(scale, maxTexturesDimension, border, pow2Textures);
    }

    // Get library version.
    public static string GetVersion()
    {
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(AmanithSVG.svgtGetString(AmanithSVG.SVGT_VERSION));
    }

    // Get library vendor.
    public static string GetVendor()
    {
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(AmanithSVG.svgtGetString(AmanithSVG.SVGT_VENDOR));
    }
    
    private static bool Init()
    {
        if (!SVGAssets.m_Initialized)
        {
            int err = AmanithSVG.svgtInit(SVGAssets.ScreenResolutionWidth, SVGAssets.ScreenResolutionHeight, SVGAssets.ScreenDpi);

            if (err != AmanithSVG.SVGT_NO_ERROR)
            {
                AmanithSVG.svgtErrorLog("Error initializing AmanithSVG the library: ", err);
                return false;
            }
            SVGAssets.m_Initialized = true;
        }
        return true;
    }

    // Keep track if AmanithSVG library has been initialized.
    private static bool m_Initialized;
}
