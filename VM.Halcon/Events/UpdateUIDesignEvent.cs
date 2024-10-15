
using HalconDotNet;
using System;
using VM.Halcon.Config;

namespace VS.Main.Events
{
    public class UpdateUIDesignEvent 
    {
    }
    public class UpdateUIDesignPar
    {
        /// <summary>
        /// 窗口ID
        /// </summary>
        public int DispViewID { get; set; }
        public Guid Guid { get; set; }
        public int CallMethodIndex { get; set; }
        public HObject obj { get; set; }
        public string color { get; set; }
        public bool isFillDisp { get; set; }
        public HText MeasROIText { get; set; }
    }
}
