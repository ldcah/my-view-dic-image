using System;
using HalconDotNet;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace MyViewROI
{

    public delegate void FuncROIDelegate();

    

    /// <summary>
    /// This class creates and manages ROI objects. It responds 
    /// to  mouse device inputs using the methods mouseDownAction and 
    /// mouseMoveAction. You don't have to know this class in detail when you 
    /// build your own C# project. But you must consider a few things if 
    /// you want to use interactive ROIs in your application: There is a
    /// quite close connection between the ROIController and the HWndCtrl 
    /// class, which means that you must 'register' the ROIController
    /// with the HWndCtrl, so the HWndCtrl knows it has to forward user input
    /// (like mouse events) to the ROIController class.  
    /// The visualization and manipulation of the ROI objects is done 
    /// by the ROIController.
    /// This class provides special support for the matching
    /// applications by calculating a model region from the list of ROIs. For
    /// this, ROIs are added and subtracted according to their sign.
    /// </summary>
    public class ROIController
    {

        public Action<string> actTip = null;

        private string Color4Active = "yellow";
        private string Color4ActiveHdl = "red";
        private string Color4Nomal = "blue";

        private string Color4Edit = "green";


        public ROI lastROI = null;
        int lastActiveROIidx = -1;




       public bool isAddRoi=false;
       public  bool isEditROI=false;


      void doActTip(string tip)
        {
            if(actTip!=null)
            {
                actTip(tip);
            }
        }



        public void doEsc()
        {
          
            if(isAddRoi)
            {
                //删除当前的ROI
                ROIList.RemoveAt(ROIList.Count-1);

                activeROIidx = -1;

                lastROI = null;

                isAddRoi = false;
            }

            if(isEditROI)
            {
                if (lastROI != null)
                {
                    lastROI.IsEditing = false;
                }
                isEditROI = false;
            }
            viewController.repaint();


            doActTip("");
        }


        public void doFinished()
        {
            if(lastROI!=null)
            {
                lastROI.IsEditing = false;
            }

            isAddRoi = false;
            isEditROI = false;

            viewController.repaint();


            doActTip("");
        }


        public void doEdit()
        {
           

            isAddRoi = false;

            if (lastROI != null)
            {
                lastROI.IsEditing = false;
            }

            //选中
            if (activeROIidx != -1)
            {
                lastROI = ROIList[activeROIidx];

                lastROI.IsEditing = true;

                isEditROI = true;


                doActTip(lastROI.doActTip4Edit());
            }

            viewController.repaint();
          

        }




        public void doDel()
        {

            if (isAddRoi || isEditROI)
            {

                var ieEdit = from v in ROIList
                             where v.IsEditing
                             select v;

                if(ieEdit!=null&& ieEdit.Count()>0)
                {
                    if (System.Windows.Forms.MessageBox.Show(string.Format("是否要删除选中的ROI"), "删除提示", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {

                        ROIList.RemoveAll(v=>v.IsEditing);

                        activeROIidx = -1;


                        lastROI = null;
                        isAddRoi = false;
                        isEditROI = false;

                        viewController.repaint();


                        doActTip("");
                    }
                }              
            }

           

        }

        public void doAddPointD()
        {
            if (isAddRoi || isEditROI)
            {
                if(lastROI!=null)
                {
                    lastROI.AddPointD();

                    viewController.repaint();
                }
            }
        }


        public void doSubPointD()
        {
            if (isAddRoi || isEditROI)
            {
                if (lastROI != null)
                {
                    lastROI.SubPointD();

                    viewController.repaint();
                }
            }
        }



        public void doAddROI(string roiType,string roiName="")
        {
            if (isAddRoi || isEditROI)
                return;
            var newRoi = (ROI)Activator.CreateInstance(Type.GetType(roiType,throwOnError:false,ignoreCase:true));
            if (newRoi == null)
                return;
            newRoi.RoiName = roiName;
            newRoi.IsEditing = true;
            setROIShape(newRoi);


            doActTip(newRoi.doActTip4Add());
        }

        /// <summary>
        /// Constant for setting the ROI mode: positive ROI sign.
        /// </summary>
        public const int MODE_ROI_POS = 21;

        /// <summary>
        /// Constant for setting the ROI mode: negative ROI sign.
        /// </summary>
        public const int MODE_ROI_NEG = 22;

        /// <summary>
        /// Constant for setting the ROI mode: no model region is computed as
        /// the sum of all ROI objects.
        /// </summary>
        public const int MODE_ROI_NONE = 23;

        /// <summary>Constant describing an update of the model region</summary>
        public const int EVENT_UPDATE_ROI = 50;

        public const int EVENT_CHANGED_ROI_SIGN = 51;

        /// <summary>Constant describing an update of the model region</summary>
        public const int EVENT_MOVING_ROI = 52;

        public const int EVENT_DELETED_ACTROI = 53;

        public const int EVENT_DELETED_ALL_ROIS = 54;

        public const int EVENT_ACTIVATED_ROI = 55;

        public const int EVENT_CREATED_ROI = 56;



        private ROI roiMode;
        private int stateROI;
        private double currX, currY;


        /// <summary>Index of the active ROI object</summary>
        public int activeROIidx;
        public int deletedIdx;

        /// <summary>List containing all created ROI objects so far</summary>
        public List<ROI> ROIList;
        [NonSerialized]
        /// <summary>
        /// Region obtained by summing up all negative 
        /// and positive ROI objects from the ROIList 
        /// </summary>
        public HRegion ModelROI;

      

        /// <summary>
        /// Reference to the HWndCtrl, the ROI Controller is registered to
        /// </summary>
        public HWndCtrl viewController;

        /// <summary>
        /// Delegate that notifies about changes made in the model region
        /// </summary>
        public IconicDelegate NotifyRCObserver;

        /// <summary>Constructor</summary>
        public ROIController()
        {
            stateROI = MODE_ROI_NONE;
            ROIList = new List<ROI>();
            activeROIidx = -1;
            ModelROI = new HRegion();
            NotifyRCObserver = new IconicDelegate(dummyI);
            deletedIdx = -1;
            currX = currY = -1;
        }

        /// <summary>Registers the HWndCtrl to this ROIController instance</summary>
        public void setViewController(HWndCtrl view)
        {
            viewController = view;
        }

        /// <summary>Gets the ModelROI object</summary>
        public HRegion getModelRegion()
        {
            return ModelROI;
        }

        /// <summary>Gets the List of ROIs created so far</summary>
        public List<ROI> getROIList()
        {
            return ROIList;
        }

        /// <summary>Get the active ROI</summary>
        public ROI getActiveROI()
        {
            if (activeROIidx != -1)
                return ROIList[activeROIidx];

            return null;
        }

        public int getActiveROIIdx()
        {
            return activeROIidx;
        }

        public void setActiveROIIdx(int active)
        {
            activeROIidx = active;
        }

        public int getDelROIIdx()
        {
            return deletedIdx;
        }

        /// <summary>
        /// To create a new ROI object the application class initializes a 
        /// 'seed' ROI instance and passes it to the ROIController.
        /// The ROIController now responds by manipulating this new ROI
        /// instance.
        /// </summary>
        /// <param name="r">
        /// 'Seed' ROI object forwarded by the application forms class.
        /// </param>
        public void setROIShape(ROI r)
        {
           
            //上一个修改的ROI
            if (lastActiveROIidx != -1)
                ROIList[activeROIidx].IsEditing = false;

            if (lastROI != null)
                lastROI.IsEditing = false;
            lastROI = r;

            isAddRoi = true;
          

            roiMode = r;
            roiMode.setOperatorFlag(stateROI);



        }





        /// <summary>
        /// Sets the sign of a ROI object to the value 'mode' (MODE_ROI_NONE,
        /// MODE_ROI_POS,MODE_ROI_NEG)
        /// </summary>
        public void setROISign(int mode)
        {
            stateROI = mode;

            if (activeROIidx != -1)
            {
                ((ROI)ROIList[activeROIidx]).setOperatorFlag(stateROI);
                viewController.repaint();
                NotifyRCObserver(ROIController.EVENT_CHANGED_ROI_SIGN);
            }
        }

        /// <summary>
        /// Removes the ROI object that is marked as active. 
        /// If no ROI object is active, then nothing happens. 
        /// </summary>
        public void removeActive()
        {
            if (activeROIidx != -1)
            {
                ROIList.RemoveAt(activeROIidx);
                deletedIdx = activeROIidx;
                activeROIidx = -1;
                viewController.repaint();
                NotifyRCObserver(EVENT_DELETED_ACTROI);
            }
        }




        /// <summary>
        /// Calculates the ModelROI region for all objects contained 
        /// in ROIList, by adding and subtracting the positive and 
        /// negative ROI objects.
        /// </summary>
        public bool defineModelROI()
        {
            HRegion tmpAdd, tmpDiff, tmp;
            double row, col;

            if (stateROI == MODE_ROI_NONE)
                return true;

            tmpAdd = new HRegion();
            tmpDiff = new HRegion();
            tmpAdd.GenEmptyRegion();
            tmpDiff.GenEmptyRegion();

            for (int i = 0; i < ROIList.Count; i++)
            {
                switch (((ROI)ROIList[i]).getOperatorFlag())
                {
                    case ROI.POSITIVE_FLAG:
                        tmp = ((ROI)ROIList[i]).getRegion();
                        tmpAdd = tmp.Union2(tmpAdd);
                        break;
                    case ROI.NEGATIVE_FLAG:
                        tmp = ((ROI)ROIList[i]).getRegion();
                        tmpDiff = tmp.Union2(tmpDiff);
                        break;
                    default:
                        break;
                }//end of switch
            }//end of for

            ModelROI = null;

            if (tmpAdd.AreaCenter(out row, out col) > 0)
            {
                tmp = tmpAdd.Difference(tmpDiff);
                if (tmp.AreaCenter(out row, out col) > 0)
                    ModelROI = tmp;
            }

            //in case the set of positiv and negative ROIs dissolve 
            if (ModelROI == null || ROIList.Count == 0)
                return false;

            return true;
        }


        /// <summary>
        /// Clears all variables managing ROI objects
        /// </summary>
        public void reset()
        {
            ROIList.Clear();
            activeROIidx = -1;
            ModelROI = null;
            roiMode = null;
            NotifyRCObserver(EVENT_DELETED_ALL_ROIS);
        }


        /// <summary>
        /// Deletes this ROI instance if a 'seed' ROI object has been passed
        /// to the ROIController by the application class.
        /// 
        /// </summary>
        public void resetROI()
        {
            activeROIidx = -1;
            roiMode = null;
        }

        /// <summary>Defines the colors for the ROI objects</summary>
        /// <param name="aColor">Color for the active ROI object</param>
        /// <param name="inaColor">Color for the inactive ROI objects</param>
        /// <param name="aHdlColor">
        /// Color for the active handle of the active ROI object
        /// </param>
        public void setDrawColor(string aColor,
                                   string aHdlColor,
                                   string inaColor)
        {
            if (aColor != "")
                Color4Active = aColor;
            if (aHdlColor != "")
                Color4ActiveHdl = aHdlColor;
            if (inaColor != "")
                Color4Nomal = inaColor;
        }


        /// <summary>
        /// Paints all objects from the ROIList into the HALCON window
        /// </summary>
        /// <param name="window">HALCON window</param>
        public void paintData(HalconDotNet.HWindow window)
        {
            window.SetDraw("margin");
            window.SetLineWidth(1);

            if (ROIList.Count > 0)
            {
                window.SetDraw("margin");

                //编辑  绿色               
                window.SetColor(Color4Edit);

                var ieRoi = from v in ROIList where v.IsEditing select v;
                foreach (var r in ieRoi)
                {
                    window.SetLineStyle(r.flagLineStyle);
                    r.draw(window);

                    window.SetColor(Color4ActiveHdl);
                    r.displayActive(window);
                }

                // 正常   蓝色
                window.SetColor(Color4Nomal);

                ieRoi = from v in ROIList where !v.IsEditing select v;             
                foreach (var r in ieRoi)
                {
                    window.SetLineStyle(r.flagLineStyle);
                    r.draw(window);
                }

                //选中 黄色
                if (activeROIidx != -1 && !ROIList[activeROIidx].IsEditing)
                {
                    window.SetColor(Color4Active);
                    window.SetLineStyle((ROIList[activeROIidx]).flagLineStyle);
                    ROIList[activeROIidx].draw(window);

                    //window.SetColor(Color4ActiveHdl);
                    //ROIList[activeROIidx].displayActive(window);
                }
            }
        }




        public void createExistingROI(HTuple RoiParameters)
        {
            if (roiMode != null)             //either a new ROI object is created
            {
                roiMode.createROI(RoiParameters);
                ROIList.Add(roiMode);
                roiMode = null;
                activeROIidx = ROIList.Count - 1;
                viewController.repaint();


                NotifyRCObserver(ROIController.EVENT_CREATED_ROI);
            }
        }



        /// <summary>
        /// Reaction of ROI objects to the 'mouse button down' event: changing
        /// the shape of the ROI and adding it to the ROIList if it is a 'seed'
        /// ROI.
        /// </summary>
        /// <param name="imgX">x coordinate of mouse event</param>
        /// <param name="imgY">y coordinate of mouse event</param>
        /// <returns></returns>
        public int mouseDownAction(double imgX, double imgY)
        {
            int idxROI = -1;
            double max = 10000, dist = 0;
            double epsilon = 35.0;          //maximal shortest distance to one of
                                            //the handles

            if (roiMode != null)             //either a new ROI object is created
            {
                roiMode.createROI(imgX, imgY);
                ROIList.Add(roiMode);
                roiMode = null;
                activeROIidx = ROIList.Count - 1;
                viewController.repaint();

                NotifyRCObserver(ROIController.EVENT_CREATED_ROI);
            }
            else if (ROIList.Count > 0)     // ... or an existing one is manipulated
            {
                activeROIidx = -1;

                for (int i = 0; i < ROIList.Count; i++)
                {
                    dist = ((ROI)ROIList[i]).distToClosestHandle(imgX, imgY);
                    if ((dist < max) && (dist < epsilon))
                    {
                        max = dist;
                        idxROI = i;
                    }
                }//end of for

                if (idxROI >= 0)
                {
                    activeROIidx = idxROI;
                    NotifyRCObserver(ROIController.EVENT_ACTIVATED_ROI);

                    if (!(isAddRoi || isEditROI))
                    {
                        doActTip("F2 编辑");
                    }
                }

                viewController.repaint();
            }
            return activeROIidx;
        }

        /// <summary>
        /// Reaction of ROI objects to the 'mouse button move' event: moving
        /// the active ROI.
        /// </summary>
        /// <param name="newX">x coordinate of mouse event</param>
        /// <param name="newY">y coordinate of mouse event</param>
        public void mouseMoveAction(double newX, double newY)
        {
            if ((newX == currX) && (newY == currY))
                return;



            //((ROI)ROIList[activeROIidx]).moveByHandle(newX, newY);

            //2021-10-22 ludc
            ROIList[activeROIidx].moveByHandleWithEidtMode(newX, newY);
            


            viewController.repaint();
            currX = newX;
            currY = newY;
            NotifyRCObserver(ROIController.EVENT_MOVING_ROI);
        }


        /***********************************************************/
        public void dummyI(int v)
        {
        }

    }//end of class
}//end of namespace
