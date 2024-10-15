﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using HalconDotNet;
using VM.Halcon.Config;
using VM.Halcon.Model;
using VS.Main.Events;

namespace VM.Halcon
{
    public class ViewWindow : IViewWindow
    {


        public void clearList()
        {
            ClearWindow();
        }

        public void repaint()
        {
            _hWndControl.Repaint();
        }

        public void addtxt(HTuple msg, HTuple row, HTuple col, string setColor = "red", string setFont = "宋体-16")
        {
            HText entry = new HText(setColor,msg,  row, col, 16,  setFont);
            DispText(entry);
           
        }

        public void addIconicVar(HObject obj, string setColor = "red", string setDraw = "margin")
        {
            if (obj == null)
                return;
            _hWndControl.DispObj(obj, setColor, setDraw== "fill");
        }



        public HWndCtrl _hWndControl;
        public ROIController _roiController;
        public Guid Guid { get; } = Guid.NewGuid();
        public bool IsUIView = false;
        /// <summary>
        /// 窗口ID
        /// </summary>
        public int DispViewID;

        public object HalconWindow { get; internal set; }

        public ViewWindow(HWindowControl window)
        {            
            this._hWndControl = new HWndCtrl(window);
            this._roiController = new ROIController();
            this._hWndControl.SetROIController(_roiController);
            this._hWndControl.SetViewState(HWndCtrl.MODE_VIEW_NONE);
        }
        /// <summary>
        /// 清空所有显示内容
        /// </summary>
        public void ClearWindow()
        {
            _hWndControl.ClearList();       //清空显示image
            _hWndControl.ClearHObjectList();//清空hobjectList
        }
        public bool fitImage_InitFlag = true;
        /// <summary>
        /// 显示图片
        /// </summary>
        /// <param name="img"></param>
        public void displayImage(HObject img,bool fitImage = false)
        {
            //TODO:20210104
            this._hWndControl.addImageShow(img);    //添加背景图片         
            this._roiController.ResetVar();         //清空roi容器,不让roi显示
            if (fitImage_InitFlag || fitImage)
            {
                fitImage_InitFlag = false;
                this._hWndControl.ResetWindow(true);      //适应图片窗口大小 要在resetWindowImage前面
            }
            this._roiController.ResetWindowImage(); //显示图片
            //this._hWndControl.ResetWindow();
            //this._hWndControl.ResetAll();
            //this._hWndControl.Repaint();
        
        }
        public void displayImageWithoutFit(HObject img)
        {
            //TODO:20210104
            this._hWndControl.addImageShow(img);    //添加背景图片         
            this._roiController.ResetVar();         //清空roi容器,不让roi显示
        }
        /// <summary>
        /// 自适应窗口
        /// </summary>
        public void resetWindow()
        {
            this._hWndControl.ResetWindow();
        }
        public void notDisplayRoi()
        {
            this._roiController.ResetVar();
            this._roiController.ResetWindowImage(); //显示图片
        }
        /// <summary>
        /// 获取当前窗口显示的roi数量
        /// </summary>
        /// <returns></returns>
        public int getRoiCount()
        {
            return _roiController.ROIList.Count;
        }
        /// <summary>
        /// 是否开启缩放事件
        /// </summary>
        /// <param name="flag"></param>
        public void setDrawModel(bool flag)
        {
            _hWndControl.drawModel = flag;
        }
        public void ResetWindowImage(bool fitImage = true)
        {
            this._hWndControl.ResetWindow(fitImage);
            this._roiController.ResetWindowImage();
        }
        public void mouseleave()
        {
            _hWndControl.RaiseMouseup();
        }
        public void zoomWindowImage()
        {
            this._roiController.zoomWindowImage();
        }
        public void moveWindowImage()
        {
            this._roiController.moveWindowImage();
        }
        public void noneWindowImage()
        {
            this._roiController.noneWindowImage();
        }
        public void genPoint(string name, string color,double beginRow, double beginCol, ref Dictionary<string, ROI> rois)
        {
            this._roiController.genPoint(name, color,beginRow, beginCol, ref rois);
        }
        public void genCircle(string name, string color, double row, double col, double radius, ref Dictionary<string, ROI> rois)
        {
            this._roiController.genCircle(name, color, row, col, radius, ref rois);
        }
        public void genCircleArr(string name, string color, double row, double col, double radius, ref Dictionary<string, ROI> rois)
        {
            this._roiController.genCircleAre(name, color, row, col, radius, ref rois);
        }
        public void genLine(string name, string color, double beginRow, double beginCol, double endRow, double endCol, ref Dictionary<string, ROI> rois)
        {
            this._roiController.genLine(name, color, beginRow, beginCol, endRow, endCol, ref rois);
        }
        public void genRect1(string name, string color, double row1, double col1, double row2, double col2, ref Dictionary<string, ROI> rois)
        {
            this._roiController.genRect1(name, color, row1, col1, row2, col2, ref rois);
        }
        public void genRect2(string name, string color, double row, double col, double phi, double length1, double length2, ref Dictionary<string, ROI> rois)
        {
            this._roiController.genRect2(name, color, row, col, phi, length1, length2, ref rois);
        }
        public void genCoordLine(string name, string color, double beginRow, double beginCol, double endRow, double endCol, ref Dictionary<string, ROI> rois)
        {
            this._roiController.genCoordLine(name, color, beginRow, beginCol, endRow, endCol, ref rois);
        }
        public ROI smallestActiveROI(out string name, out string index)
        {
            ROI resual = this._roiController.smallestActiveROI(out name, out index);
            return resual;
        }
        public ROI smallestActiveROI(out List<double> data, out string index)
        {
            ROI roi = this._roiController.smallestActiveROI(out data, out index);
            return roi;
        }
        public void selectROI(string index)
        {
            this._roiController.selectROI(index);
        }
        public void selectROI(Dictionary<string, ROI> rois, string index)
        {
            //this._roiController.selectROI(index);
            foreach (KeyValuePair<string, ROI> roi in rois)
            {
                this._hWndControl.ResetAll();
                this._hWndControl.Repaint();
                HTuple m_roiData = null;
                m_roiData = rois[index].GetModelData();
                switch (rois[index].Type)
                {
                    case ROIType.Rectangle1:
                        if (m_roiData != null)
                        {
                            this._roiController.displayRect1(roi.Key, rois[index].Color, m_roiData[0].D, m_roiData[1].D, m_roiData[2].D, m_roiData[3].D);
                        }
                        break;
                    case ROIType.Rectangle2:
                        if (m_roiData != null)
                        {
                            this._roiController.displayRect2(roi.Key, rois[index].Color, m_roiData[0].D, m_roiData[1].D, m_roiData[2].D, m_roiData[3].D, m_roiData[4].D);
                        }
                        break;
                    case ROIType.Circle:
                        if (m_roiData != null)
                        {
                            this._roiController.displayCircle(roi.Key, rois[index].Color, m_roiData[0].D, m_roiData[1].D, m_roiData[2].D);
                        }
                        break;
                    case ROIType.Line:
                        if (m_roiData != null)
                        {
                            this._roiController.displayLine(roi.Key, rois[index].Color, m_roiData[0].D, m_roiData[1].D, m_roiData[2].D, m_roiData[3].D);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public void DispROI(Dictionary<string, ROI> rois)
        {
            if (rois == null)
            {
                return;
            }
            //this._hWndControl.resetAll();
            //this._hWndControl.repaint();
            foreach (KeyValuePair<string, ROI> roi in rois)
            {
                HTuple m_roiData = null;
                m_roiData = roi.Value.GetModelData();
                switch (roi.Value.Type)
                {
                    case ROIType.Point:
                        if (m_roiData != null)
                        {
                            this._roiController.displayPoint(roi.Key, roi.Value.Color, m_roiData[0].D, m_roiData[1].D);
                        }
                        break;
                    case ROIType.Circle:
                        if (m_roiData != null)
                        {
                            this._roiController.displayCircle(roi.Key, roi.Value.Color, m_roiData[0].D, m_roiData[1].D, m_roiData[2].D);
                        }
                        break;
                    case ROIType.Line:
                        if (m_roiData != null)
                        {
                            this._roiController.displayLine(roi.Key, roi.Value.Color, m_roiData[0].D, m_roiData[1].D, m_roiData[2].D, m_roiData[3].D);
                        }
                        break;
                    case ROIType.Rectangle1:
                        if (m_roiData != null)
                        {
                            this._roiController.displayRect1(roi.Key, roi.Value.Color, m_roiData[0].D, m_roiData[1].D, m_roiData[2].D, m_roiData[3].D);
                        }
                        break;
                    case ROIType.Rectangle2:
                        if (m_roiData != null)
                        {
                            this._roiController.displayRect2(roi.Key, roi.Value.Color, m_roiData[0].D, m_roiData[1].D, m_roiData[2].D, m_roiData[3].D, m_roiData[4].D);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public void DispROI(string name, ROI roi)
        {
            if (roi == null)
            {
                return;
            }
            HTuple m_roiData = roi.GetModelData();
            switch (roi.Type)
            {
                case ROIType.Point:
                    if (m_roiData != null)
                    {
                        this._roiController.displayPoint(name, roi.Color, m_roiData[0].D, m_roiData[1].D);
                    }
                    break;
                case ROIType.Circle:
                    if (m_roiData != null)
                    {
                        this._roiController.displayCircle(name, roi.Color, m_roiData[0].D, m_roiData[1].D, m_roiData[2].D);
                    }
                    break;
                case ROIType.Line:
                    if (m_roiData != null)
                    {
                        this._roiController.displayLine(name, roi.Color, m_roiData[0].D, m_roiData[1].D, m_roiData[2].D, m_roiData[3].D);
                    }
                    break;
                case ROIType.Rectangle1:
                    if (m_roiData != null)
                    {
                        this._roiController.displayRect1(name, roi.Color, m_roiData[0].D, m_roiData[1].D, m_roiData[2].D, m_roiData[3].D);
                    }
                    break;
                case ROIType.Rectangle2:
                    if (m_roiData != null)
                    {
                        this._roiController.displayRect2(name, roi.Color, m_roiData[0].D, m_roiData[1].D, m_roiData[2].D, m_roiData[3].D, m_roiData[4].D);
                    }
                    break;
                case ROIType.CoordLine:
                    if (m_roiData != null)
                    {
                        this._roiController.displayCoordLine(name, roi.Color, m_roiData[0].D, m_roiData[1].D, m_roiData[2].D, m_roiData[3].D);
                    }
                    break;
            }
        }
        public void removeActiveROI(ref Dictionary<string, ROI> rois)
        {
            this._roiController.removeActiveROI(ref rois);
        }
        public void setActiveRoi(string index)
        {
            this._roiController.ActiveROIId = index;
        }
        public void saveROI(Dictionary<string, ROI> rois, string fileNmae)
        {
            List<ROIInfo> m_RoiData = new List<ROIInfo>();
            foreach (KeyValuePair<string, ROI> kvp in rois)
            {
                m_RoiData.Add(new ROIInfo(kvp.Key, kvp.Value));
            }
            SerializeHelper.Save(m_RoiData, fileNmae);
        }
        public void loadROI(string fileName, out Dictionary<string, ROI> rois)
        {
            rois = new Dictionary<string, ROI>();
            //List<ROIInfo> m_RoiData = new List<ROIInfo>();
            //m_RoiData = (List<ROIInfo>)SerializeHelper.Load(m_RoiData.GetType(), fileName);
            //for (int i = 0; i < m_RoiData.Count; i++)
            //{
            //    switch (m_RoiData[i].Name)
            //    {
            //        case "Rectangle1":
            //            this._roiController.genRect1("",m_RoiData[i].Rectangle1.Row1, m_RoiData[i].Rectangle1.Column1,m_RoiData[i].Rectangle1.Row2, m_RoiData[i].Rectangle1.Column2, ref rois);
            //            break;
            //        case "Rectangle2":
            //            this._roiController.genRect2(m_RoiData[i].Rectangle2.Row, m_RoiData[i].Rectangle2.Column,
            //                m_RoiData[i].Rectangle2.Phi, m_RoiData[i].Rectangle2.Lenth1, m_RoiData[i].Rectangle2.Lenth2, ref rois);
            //            rois.Last().Color = m_RoiData[i].Rectangle2.Color;
            //            break;
            //        case "Circle":
            //            this._roiController.genCircle(m_RoiData[i].Circle.Row, m_RoiData[i].Circle.Column, m_RoiData[i].Circle.Radius, ref rois);
            //            rois.Last().Color = m_RoiData[i].Circle.Color;
            //            break;
            //        case "Line":
            //            this._roiController.genLine(m_RoiData[i].Line.RowBegin, m_RoiData[i].Line.ColumnBegin,
            //                m_RoiData[i].Line.RowEnd, m_RoiData[i].Line.ColumnEnd, ref rois);
            //            rois.Last().Color = m_RoiData[i].Line.Color;
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //this._hWndControl.ResetAll();
            //this._hWndControl.Repaint();
        }
        #region 专门用于 显示region 和xld的方法
        public void DispHobject(HObject obj, string color,bool isFillDisp)
        {
            _hWndControl.DispObj(obj, color, isFillDisp);
         
        }
        public void DispHobject(HObject obj, string color)
        {
            _hWndControl.DispObj(obj, color, false);
        
        }
        public void DispHobject(HObject obj,bool isFillDisp)
        {
            _hWndControl.DispObj(obj, null, isFillDisp);
       
        }
        public void DispText(HText MeasROIText)
        {
            _hWndControl.DispObj(MeasROIText);
          
        }

        public void ResetWindowImage()
        {
        }
        #endregion
    }
}
