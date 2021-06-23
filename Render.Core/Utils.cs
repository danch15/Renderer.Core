using System;
using SlimDX;
using SlimDX.Direct3D9;

namespace Renderer.Core
{
    class Utils
    {
        public static void ApplyLetterBoxing(ref RECT rendertTargetArea, float frameWidth, float frameHeight, double aspectRatio)
        {
            float ratio = frameWidth / frameHeight;
	        if(aspectRatio > 0)
	        {
		        ratio *= (float)aspectRatio;
	        }
            
            float targetW = Math.Abs((float)(rendertTargetArea.Right - rendertTargetArea.Left));
            float targetH = Math.Abs((float)(rendertTargetArea.Bottom - rendertTargetArea.Top));
            float tempH = targetW / ratio;              
            if(tempH <= targetH)
            {
                float deltaH = Math.Abs(tempH - targetH) / 2;
                rendertTargetArea.Top += (int)deltaH;
                rendertTargetArea.Bottom -= (int)deltaH;
            }
            else
            {
                float tempW = targetH * ratio;
                float deltaW = Math.Abs(tempW - targetW) / 2;
                rendertTargetArea.Left += (int)deltaW;
                rendertTargetArea.Right -= (int)deltaW;
            }
        }


    }
}
