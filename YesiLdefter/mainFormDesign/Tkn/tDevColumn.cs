using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.IO;


using WIA;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraDataLayout;
using DevExpress.XtraLayout;

using Tkn_CreateObject;
using Tkn_Forms;
using Tkn_DefaultValue;
using Tkn_Events;
using Tkn_ToolBox;
using Tkn_Variable;
using Tkn_Save;

namespace Tkn_DevColumn
{
    public class tDevColumn:tBase
    {

        #region RepositoryItemImageComboBox, RepositoryItemRadioGroup
        
        public void RepositoryItemImageComboBox_Fill(RepositoryItemImageComboBox ItemBox, DataRow Row, string default_value, byte tview_type)
        {
            tToolBox t = new tToolBox();

            string List_Name = t.Set(Row["LIST_TYPES_NAME"].ToString(), Row["LKP_LIST_TYPES_NAME"].ToString(), "");

            if (t.IsNotNull(List_Name))
                tRepositoryItem_Fill(ItemBox, null, null, null, null, null, List_Name, default_value, tview_type);
        }

        public void XtraEditorsImageComboBox_Fill(ImageComboBoxEdit ItemBox, DataRow Row, string default_value, byte tview_type)
        {
            tToolBox t = new tToolBox();

            string List_Name = t.Set(Row["LIST_TYPES_NAME"].ToString(), Row["LKP_LIST_TYPES_NAME"].ToString(), "");

            if (t.IsNotNull(List_Name))
                tRepositoryItem_Fill(null, ItemBox, null, null, null, null, List_Name, default_value, tview_type);

        }

        public void RepositoryItemComboBox_Fill(RepositoryItemComboBox ItemBox, DataRow Row, string default_value, byte tview_type)
        {
            tToolBox t = new tToolBox();
            
            string List_Name = t.Set(Row["LIST_TYPES_NAME"].ToString(), Row["LKP_LIST_TYPES_NAME"].ToString(), "");

            if (t.IsNotNull(List_Name))
                tRepositoryItem_Fill(null, null, ItemBox, null, null, null, List_Name, default_value, tview_type);
        }

        public void XtraEditorsComboBox_Fill(DevExpress.XtraEditors.ComboBoxEdit ItemBox, DataRow Row, string default_value, byte tview_type)
        {
            tToolBox t = new tToolBox();

            string List_Name = t.Set(Row["LIST_TYPES_NAME"].ToString(), Row["LKP_LIST_TYPES_NAME"].ToString(), "");

            if (t.IsNotNull(List_Name))
                tRepositoryItem_Fill(null, null, null, ItemBox, null, null, List_Name, default_value, tview_type);

        }
        
        #endregion 

        #region RadioGroup

        public void RepositoryItemRadioGroup_Fill(RadioGroup ItemBox, DataRow Row, string default_value, byte tview_type)
        {
            tToolBox t = new tToolBox();

            string List_Name = t.Set(Row["LIST_TYPES_NAME"].ToString(), Row["LKP_LIST_TYPES_NAME"].ToString(), "");

            if (t.IsNotNull(List_Name))
                tRepositoryItem_Fill(null, null, null, null, null, ItemBox, List_Name, default_value, tview_type);
        }

        public void RepositoryItemRadioGroup_Fill(RepositoryItemRadioGroup ItemBox, DataRow Row, string default_value, byte tview_type)
        {
            tToolBox t = new tToolBox();

            string List_Name = t.Set(Row["LIST_TYPES_NAME"].ToString(), Row["LKP_LIST_TYPES_NAME"].ToString(), "");

            if (t.IsNotNull(List_Name))
                tRepositoryItem_Fill(null, null, null, null, ItemBox, null, List_Name, default_value, tview_type);
        }

        #endregion

        #region sub RepositoryItemImageComboBox, RepositoryItemRadioGroup

        private DataSet Find_TypesList(string ListName)
        {
            int i = -1;

            

            i = v.ds_TypesNames.IndexOf(ListName);

            if (i > -1)
            {
                return v.ds_TypesList;
            }

            i = v.ds_MsTypesNames.IndexOf(ListName);

            if (i > -1)
            {
                return v.ds_MsTypesList;
            }

            return null;
        }

        public void tRepositoryItem_Fill(RepositoryItemComboBox ItemBox_ICB,
                                         DevExpress.XtraEditors.ImageComboBoxEdit tEdit_ICB,
                                         RepositoryItemComboBox ItemBox_CB,
                                         DevExpress.XtraEditors.ComboBoxEdit tEdit_CB,      
                                         RepositoryItemRadioGroup ItemBox_RG,
                                         DevExpress.XtraEditors.RadioGroup tEdit_RG,
                                         string List_Name,
                                         string default_value,
                                         byte tview_type )
        {
            DataSet dsTypesList = Find_TypesList(List_Name);

            if (dsTypesList == null) return;
            if (dsTypesList.Tables.Count == 0) return;

            string lstname = string.Empty;
            string caption = string.Empty;
            string value = string.Empty;
            Int16 type = 0;
            Int16 image_id = 0;
            bool onay = true;

            // tview_type 
            // 1 = Normal Liste
            // 2 = Kriterler için Tümü ibaresi eklenmiş

            int i1 = dsTypesList.Tables[0].Rows.Count;
            int i2 = 0;
            for (int i = 0; i < i1; i++)
            {
                lstname = dsTypesList.Tables[0].Rows[i]["TYPES_NAME"].ToString();
                //image_id = Convert.ToInt16(dsTypesList.Tables[0].Rows[i]["IMAGE_ID"].ToString());

                image_id = -1;
                if (dsTypesList.Tables[0].Rows[i]["VALUE_INT"].ToString() != "")
                    image_id = Convert.ToInt16(dsTypesList.Tables[0].Rows[i]["VALUE_INT"].ToString());

                onay = true;

                if ((List_Name == lstname) && (image_id != -9)) // Sys_Types_T değil ise 
                {
                    type = Convert.ToInt16(dsTypesList.Tables[0].Rows[i]["VALUE_TYPE"].ToString());

                    if ((i2 == 0) && (tview_type == 2))
                        RepositoryItem_Add_(
                            ItemBox_ICB, tEdit_ICB,
                            ItemBox_CB, tEdit_CB,
                            ItemBox_RG, tEdit_RG, 
                            null, null, "Tümü", "-2", type);

                    caption = dsTypesList.Tables[0].Rows[i]["VALUE_CAPTION"].ToString();

                    if ((i2 == 0) && (tview_type == 2) && caption == "") onay = false;

                    if (onay)
                    {
                        if ((type == 1) || (type == 2) || (type == 5))
                            value = dsTypesList.Tables[0].Rows[i]["VALUE_INT"].ToString();
                        if ((type == 3) || (type == 4))
                            value = dsTypesList.Tables[0].Rows[i]["VALUE_STR"].ToString();

                        RepositoryItem_Add_(
                            ItemBox_ICB, tEdit_ICB,
                            ItemBox_CB, tEdit_CB,
                            ItemBox_RG, tEdit_RG, 
                            null, null, caption, value, type);
                    }

                    i2++;
                }

                //MS_VARIABLES
                if ((List_Name == lstname) && (image_id == -9)) // Sys_Types_T ise 
                {
                    Sys_Types_T_Read(dsTypesList, i, ItemBox_ICB, tEdit_ICB, null, null);
                    break;
                }

                if ((List_Name != lstname) && (i2 > 0)) break;

            } // for

            // SpeedKriterler için
            // default value var ise set ediliyor
            if (default_value != "")
            {
                if (tEdit_ICB != null)
                {
                    if (type == 1) tEdit_ICB.EditValue = Convert.ToInt16(default_value);
                    if (type == 2) tEdit_ICB.EditValue = Convert.ToInt32(default_value);
                    if (type == 3) tEdit_ICB.EditValue = default_value;
                    if (type == 4)
                    {
                        if (default_value == "-2") tEdit_ICB.EditValue = default_value;
                        else tEdit_ICB.EditValue = Convert.ToBoolean(default_value);
                    }
                }
                if (ItemBox_ICB != null)
                {
                    //if (type == 1) ItemBox_ICB.  //[Convert.ToInt16(default_value)];
                    //if (type == 2) ItemBox_ICB.EditValue = Convert.ToInt32(default_value);
                    //if (type == 3) ItemBox_ICB.EditValue = default_value;
                    //if (type == 4) ItemBox_ICB.EditValue = default_value;
                }
            }

        }

        private void Sys_Types_T_Read(DataSet ds, int pos,
                                      RepositoryItemComboBox ItemBox_ICB,
                                      ImageComboBoxEdit tEdit_ICB,
                                      RepositoryItemCheckedComboBoxEdit ItemBox_CCmb,
                                      CheckedComboBoxEdit tEdit_CCmb)
        {
            tToolBox t = new tToolBox();
            string Sql = string.Empty;

            // LKP_TABLE olan yani kullanıcı tarafından girilen Hesap Planları listeleri 

            string tableName = t.Set(ds.Tables[0].Rows[pos]["LKP_TABLE_NAME"].ToString(), "", "");
            string val_type_ = t.Set(ds.Tables[0].Rows[pos]["VALUE_TYPE"].ToString(), "", "");
            string str_fname = t.Set(ds.Tables[0].Rows[pos]["VALUE_STR"].ToString(), "", "null");
            string int_fname = t.Set(ds.Tables[0].Rows[pos]["VALUE_INT_FNAME"].ToString(), "", "null");
            string cap_fname = t.Set(ds.Tables[0].Rows[pos]["VALUE_CAPTION"].ToString(), "", "");
            string where = t.Set(ds.Tables[0].Rows[pos]["WHERE_SQL"].ToString(), "", "");
            string orderby = t.Set(ds.Tables[0].Rows[pos]["ORDER_BY"].ToString(), "", "");
            if (t.IsNotNull(where)) where = " where 0 = 0 " + where;

            Sql = " select "
            + "   " + val_type_ + "  VALUE_TYPE "
            + " , " + int_fname + "  VALUE_INT "
            + " , " + str_fname + "  VALUE_STR "
            + " , " + cap_fname + "  VALUE_CAPTION "
            + " from " + tableName + "  "
            + where
            ;

            if (t.IsNotNull(orderby))
            {
                if (orderby.IndexOf("order by") == -1)
                    orderby = " order by " + orderby;

                Sql = Sql + v.ENTER + orderby;
            }

            DataSet dsData = new DataSet();

            if ((Sql.IndexOf("[MSV3]") > -1) | (Sql.IndexOf("MSV3") > -1))
                 t.SQL_Read_Execute(v.dBaseNo.Manager, dsData, ref Sql, "LKP_TABLE_" + tableName, "");
            else t.SQL_Read_Execute(v.dBaseNo.Project, dsData, ref Sql, "LKP_TABLE_" + tableName, "");

            string caption = string.Empty;
            string value = string.Empty;
            Int16 type = 0;

            // İlk satırdaki boş satır
            if ((ItemBox_ICB != null) || (tEdit_ICB != null))
                RepositoryItem_Add_(ItemBox_ICB, tEdit_ICB, null, null, null, null, null, null, "", "0", 2);

            int i1 = dsData.Tables[0].Rows.Count;
            for (int i = 0; i < i1; i++)
            {
                type = Convert.ToInt16(dsData.Tables[0].Rows[i]["VALUE_TYPE"].ToString());
                caption = dsData.Tables[0].Rows[i]["VALUE_CAPTION"].ToString();
                //if ((type == 1) || (type == 2))
                value = dsData.Tables[0].Rows[i]["VALUE_INT"].ToString();
                //if ((type == 3) || (type == 4))
                //    value = dsData.Tables[0].Rows[i]["VALUE_STR"].ToString();

                if ((ItemBox_ICB != null) || (tEdit_ICB != null) ||
                    (ItemBox_CCmb != null) || (tEdit_CCmb != null))
                    RepositoryItem_Add_(ItemBox_ICB, tEdit_ICB, null, null, null, null, ItemBox_CCmb, tEdit_CCmb, caption, value, type);
            }

        }

        private void RepositoryItem_Add_(RepositoryItemComboBox ItemBox_ICB,
                                         DevExpress.XtraEditors.ImageComboBoxEdit tEdit_ICB, //-- 
                                         RepositoryItemComboBox ItemBox_CB,
                                         DevExpress.XtraEditors.ComboBoxEdit tEdit_CB,       //--
                                         RepositoryItemRadioGroup ItemBox_RG,
                                         DevExpress.XtraEditors.RadioGroup tEdit_RG,         //--
                                         RepositoryItemCheckedComboBoxEdit ItemBox_CCmb,
                                         DevExpress.XtraEditors.CheckedComboBoxEdit tEdit_CCmb, //--
                                         string caption, string value, Int16 type)
        {

            // Byte değilse
            if (type != 4)
            {
                if (ItemBox_CCmb != null)
                {
                    ItemBox_CCmb.Items.Add("[" + value + "] " + caption, CheckState.Unchecked, true);
                    return;
                }

                if (tEdit_CCmb != null)
                {
                    tEdit_CCmb.Properties.Items.Add("[" + value + "] " + caption, CheckState.Unchecked, true);
                    return;
                }
            }

            // Numeric   ToDecimal
            if (type == 5)
            {
                if (ItemBox_ICB != null)
                    ItemBox_ICB.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(caption, Convert.ToDecimal(value), type));
                if (tEdit_ICB != null)
                    tEdit_ICB.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(caption, Convert.ToDecimal(value), type));

                if (ItemBox_RG != null)
                    ItemBox_RG.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(Convert.ToDecimal(value), caption));
                if (tEdit_RG != null)
                    tEdit_RG.Properties.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(Convert.ToDecimal(value), caption));

                return;
            }

            // SmallInt
            if (type == 1)
            {
                if (ItemBox_ICB != null)
                    ItemBox_ICB.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(caption, Convert.ToInt16(value), type));
                if (tEdit_ICB != null)
                    tEdit_ICB.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(caption, Convert.ToInt16(value), type));

                if (ItemBox_RG != null)
                    ItemBox_RG.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(Convert.ToInt16(value), caption));
                if (tEdit_RG != null)
                    tEdit_RG.Properties.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(Convert.ToInt16(value), caption));
                return;
            }
            // Int
            if (type == 2)
            {
                if (ItemBox_ICB != null)
                    ItemBox_ICB.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(caption, Convert.ToInt32(value), type));
                if (tEdit_ICB != null)
                    tEdit_ICB.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(caption, Convert.ToInt32(value), type));

                if (ItemBox_RG != null)
                    ItemBox_RG.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(Convert.ToInt32(value), caption));
                if (tEdit_RG != null)
                    tEdit_RG.Properties.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(Convert.ToInt32(value), caption));

                return;
            }
            // string
            if (type == 3)
            {
                if (ItemBox_ICB != null)
                    ItemBox_ICB.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(caption, value, type));
                if (tEdit_ICB != null)
                    tEdit_ICB.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(caption, value, type));

                if (ItemBox_RG != null)
                    ItemBox_RG.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(value, caption));
                if (tEdit_RG != null)
                    tEdit_RG.Properties.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(value, caption));

                return;
            }
            // Byte
            if (type == 4)
            {
                if (value == "-2") // Tümü işareti ise
                {
                    if (ItemBox_ICB != null)
                        ItemBox_ICB.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(caption, value, type));
                    if (tEdit_ICB != null)
                        tEdit_ICB.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(caption, value, type));
                }

                if (value != "-2") // Tümü işareti değilse
                {
                    if (ItemBox_ICB != null)
                        ItemBox_ICB.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(caption, Convert.ToBoolean(value), type));
                    if (tEdit_ICB != null)
                        tEdit_ICB.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(caption, Convert.ToBoolean(value), type));

                    if (ItemBox_RG != null)
                        ItemBox_RG.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(Convert.ToBoolean(value), caption));
                    if (tEdit_RG != null)
                        tEdit_RG.Properties.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(Convert.ToBoolean(value), caption));
                }
                return;
            }

            // ComboBox'lar ise
            if (ItemBox_CB != null)
                ItemBox_CB.Items.Add(new DevExpress.XtraEditors.Controls.ComboBoxItem(caption));
            if (tEdit_CB != null)
                tEdit_CB.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ComboBoxItem(caption));
            
        }

        #endregion sub RepositoryItemImageComboBox, RepositoryItemRadioGroup

        #region RepositoryItemCheckedComboBoxEdit

        public void RepositoryItemCheckedComboBoxEdit_Fill(CheckedComboBoxEdit ItemBox, DataRow Row)
        {
            tToolBox t = new tToolBox();

            string List_Name = t.Set(Row["LIST_TYPES_NAME"].ToString(), Row["LKP_LIST_TYPES_NAME"].ToString(), "");

            if (t.IsNotNull(List_Name))
                RepositoryItemCheckedComboBoxEdit_Fill_(null, ItemBox, List_Name);
        }

        public void RepositoryItemCheckedComboBoxEdit_Fill(RepositoryItemCheckedComboBoxEdit ItemBox, DataRow Row)
        {
            tToolBox t = new tToolBox();

            string List_Name = t.Set(Row["LIST_TYPES_NAME"].ToString(), Row["LKP_LIST_TYPES_NAME"].ToString(), "");

            if (t.IsNotNull(List_Name))
                RepositoryItemCheckedComboBoxEdit_Fill_(ItemBox, null, List_Name);
        }

        public void RepositoryItemCheckedComboBoxEdit_Fill_(
                       RepositoryItemCheckedComboBoxEdit ItemBox, 
                       CheckedComboBoxEdit tEdit, 
                       string List_Name)
        {
            DataSet dsTypesList = Find_TypesList(List_Name);

            if (dsTypesList == null) return;
            if (dsTypesList.Tables.Count == 0) return;

            tToolBox t = new tToolBox();

            string lstname = string.Empty;
            string caption = string.Empty;
            string value = string.Empty;
            Int16 type = 0;
            Int16 image_id = 0;

            int i1 = dsTypesList.Tables[0].Rows.Count;
            int i2 = 0;
            for (int i = 0; i < i1; i++)
            {
                lstname = dsTypesList.Tables[0].Rows[i]["TYPES_NAME"].ToString();
                //image_id = Convert.ToInt16(dsTypesList.Tables[0].Rows[i]["IMAGE_ID"].ToString());

                image_id = -1;
                if (dsTypesList.Tables[0].Rows[i]["VALUE_INT"].ToString() != "")
                    image_id = Convert.ToInt16(dsTypesList.Tables[0].Rows[i]["VALUE_INT"].ToString());


                if ((List_Name == lstname) && (image_id != -9)) // Sys_Types_T değil ise 
                {
                    type = Convert.ToInt16(dsTypesList.Tables[0].Rows[i]["VALUE_TYPE"].ToString());
                    caption = dsTypesList.Tables[0].Rows[i]["VALUE_CAPTION"].ToString();

                    if ((type == 1) || (type == 2) || (type == 5))
                        value = dsTypesList.Tables[0].Rows[i]["VALUE_INT"].ToString();
                    if ((type == 3) || (type == 4))
                        value = dsTypesList.Tables[0].Rows[i]["VALUE_STR"].ToString();

                    if (t.IsNotNull(value))
                    {
                        if (ItemBox != null)
                        {
                            ItemBox.Items.Add("[" + value + "] " + caption, CheckState.Unchecked, true);
                        }

                        if (tEdit != null)
                        {
                            tEdit.Properties.Items.Add("[" + value + "] " + caption, CheckState.Unchecked, true);
                        }
                    }
                    i2++;
                }

                if ((List_Name == lstname) && (image_id == -9)) // Sys_Types_T ise 
                {
                    Sys_Types_T_Read(dsTypesList, i, null, null, ItemBox, tEdit);
                    break;
                }

                if ((List_Name != lstname) && (i2 > 0)) break;

            }

            if (ItemBox != null)
            {
                // Specify the separator character.
                ItemBox.SeparatorChar = ',';
            }

            if (tEdit != null)
            {
                // Specify the separator character.
                tEdit.Properties.SeparatorChar = ',';
            }
            
        }

        #endregion RepositoryItemCheckedComboBoxEdit

        #region DisplayFormat

        public void RepositoryItem_DisplayFormat(RepositoryItemTextEdit tEdit, string tdisplayformat, Int16 tcmp_format_type)
        {
            tToolBox t = new tToolBox();

            #region displayformat
            if (t.IsNotNull(tdisplayformat))
            {
                //-1,  'not' 
                // 0,  'none'
                // 1,  'Numeric'
                // 2,  'DateTime'
                // 3,  'Custom'

                //Int16 tcmp_format_type = t.Set(Row["CMP_FORMAT_TYPE"].ToString(), Row["LKP_CMP_FORMAT_TYPE"].ToString(), (Int16)0);

                if ((tcmp_format_type == 0) || (tcmp_format_type == 1))
                {
                    //tEdit.RightToLeft = RightToLeft.Yes;
                    tEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                }
                if (tcmp_format_type == 2) tEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
                if (tcmp_format_type == 3) tEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Custom;

                tEdit.Mask.EditMask = tdisplayformat;
                tEdit.Mask.SaveLiteral = true;
                tEdit.Mask.ShowPlaceHolders = true;
                tEdit.Mask.UseMaskAsDisplayFormat = true;

                tEdit.DisplayFormat.FormatString = tdisplayformat;
                if ((tcmp_format_type == 0) || (tcmp_format_type == 1))
                    tEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                if (tcmp_format_type == 2) tEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                if (tcmp_format_type == 3) tEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

                tEdit.EditFormat.FormatString = tdisplayformat;
                if ((tcmp_format_type == 0) || (tcmp_format_type == 1))
                    tEdit.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                if (tcmp_format_type == 2) tEdit.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                if (tcmp_format_type == 3) tEdit.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;

            }
            #endregion displayformat
        }

        public void XtraEditorsTextEdit_DisplayFormat(DevExpress.XtraEditors.TextEdit tEdit, string tdisplayformat, Int16 tcmp_format_type)
        {
            tToolBox t = new tToolBox();

            #region displayformat
            if (t.IsNotNull(tdisplayformat))
            {
                //-1,  'not' 
                // 0,  'none'
                // 1,  'Numeric'
                // 2,  'DateTime'
                // 3,  'Custom'

                //tcmp_format_type = t.Set(row_Fields["CMP_FORMAT_TYPE"].ToString(), row_Fields["LKP_CMP_FORMAT_TYPE"].ToString(), (Int16)0);

                if ((tcmp_format_type == 0) || (tcmp_format_type == 1))
                {
                    tEdit.RightToLeft = RightToLeft.Yes;
                    tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                }
                if (tcmp_format_type == 2) tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
                if (tcmp_format_type == 3) tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Custom;

                tEdit.Properties.Mask.EditMask = tdisplayformat;
                tEdit.Properties.Mask.SaveLiteral = true;
                tEdit.Properties.Mask.ShowPlaceHolders = true;
                tEdit.Properties.Mask.UseMaskAsDisplayFormat = true;

                /*
                tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Custom;
                tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
                tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
                tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
                tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
                tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Regular;
                tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
                */

                tEdit.Properties.DisplayFormat.FormatString = tdisplayformat;
                if ((tcmp_format_type == 0) || (tcmp_format_type == 1))
                    tEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                if (tcmp_format_type == 2) tEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                if (tcmp_format_type == 3) tEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

                tEdit.Properties.EditFormat.FormatString = tdisplayformat;
                if ((tcmp_format_type == 0) || (tcmp_format_type == 1))
                    tEdit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                if (tcmp_format_type == 2) tEdit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                if (tcmp_format_type == 3) tEdit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;

            }
            #endregion
        }
    
        #endregion DisplayFormat
        
        //----------------------------------------------------------

        #region Grid_ColumnEdit

        public void Grid_ColumnEdit(DataRow Row, GridColumn Column, string tcolumn_type, string TableIPCode)
        {
            tToolBox t = new tToolBox();
            tEvents ev = new tEvents();

            string tTableName = Row["LKP_TABLE_NAME"].ToString();
            string tFieldName = Row["LKP_FIELD_NAME"].ToString();
            string tExpression = Row["LKP_PROP_EXPRESSION"].ToString();
            // formülleri çalıştırmak için etkisi varmı
            // 0 = yok
            // 1 = true
            Int16 tExpressionType = t.myInt16(Row["LKP_EXPRESSION_TYPE"].ToString());
            string tdisplayformat = t.Set(Row["CMP_DISPLAY_FORMAT"].ToString(), Row["LKP_CMP_DISPLAY_FORMAT"].ToString(), "");
            string tProp_Navigator = t.Set(Row["PROP_NAVIGATOR"].ToString(), "", "");
            string prop_ = tProp_Navigator.Replace((char)34, (char)39);



            #region tPropertiesEdit
            if (tcolumn_type == "tPropertiesEdit")
            {
                RepositoryItemButtonEdit tEdit = new RepositoryItemButtonEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                tEdit.KeyDown += new KeyEventHandler(ev.buttonEdit_KeyDown);
                tEdit.KeyPress += new KeyPressEventHandler(ev.buttonEdit_KeyPress);
                tEdit.KeyUp += new KeyEventHandler(ev.buttonEdit_KeyUp);
                tEdit.Leave += new EventHandler(ev.buttonEdit_Leave);
                tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);
                
                Int16 width = t.Set(Row["LKP_CMP_WIDTH"].ToString(), "", (Int16)200);

                string s = string.Empty;
                t.MyProperties_Set(ref s, "Type", v.Properties);
                t.MyProperties_Set(ref s, "TableName", tTableName);
                t.MyProperties_Set(ref s, "FieldName", tFieldName);
                t.MyProperties_Set(ref s, "Width", width.ToString());

                tEdit.AccessibleDescription = s;
                
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region tPropertiesPlusEdit
            if (tcolumn_type == "tPropertiesPlusEdit")
            {
                RepositoryItemButtonEdit tEdit = new RepositoryItemButtonEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                tEdit.KeyDown += new KeyEventHandler(ev.buttonEdit_KeyDown);
                tEdit.KeyPress += new KeyPressEventHandler(ev.buttonEdit_KeyPress);
                tEdit.KeyUp += new KeyEventHandler(ev.buttonEdit_KeyUp);
                tEdit.Leave += new EventHandler(ev.buttonEdit_Leave);
                tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);

                Int16 width = t.Set(Row["LKP_CMP_WIDTH"].ToString(), "", (Int16)200);

                string s = string.Empty;
                t.MyProperties_Set(ref s, "Type", v.PropertiesPlus);// Properties);
                t.MyProperties_Set(ref s, "TableName", tTableName);
                t.MyProperties_Set(ref s, "FieldName", tFieldName);
                t.MyProperties_Set(ref s, "Width", width.ToString());

                tEdit.AccessibleDescription = s;
               
                Column.ColumnEdit = tEdit;
            }
            #endregion
            
            #region ButtonEdit / tSearchEdit
            if ((tcolumn_type == "ButtonEdit") ||
                (tcolumn_type == "tSearchEdit"))
            {
                RepositoryItemButtonEdit tEdit = new RepositoryItemButtonEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.AccessibleName = TableIPCode;
                tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                //tEdit.KeyDown += new KeyEventHandler(ev.buttonEdit_KeyDown);
                tEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myRepositoryItemEdit_KeyDown);
                tEdit.KeyPress += new KeyPressEventHandler(ev.buttonEdit_KeyPress);
                tEdit.KeyUp += new KeyEventHandler(ev.buttonEdit_KeyUp);
                tEdit.Leave += new EventHandler(ev.buttonEdit_Leave);
                tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);
                
                // SİLME ÇALIŞIYOR
                if (tFieldName == "LKP_KOMUT")
                {
                    tEdit.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.OK;
                    tEdit.Buttons[0].Width = 40; // Column.Width / 2;
                    tEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                }
                                
                #region tSearchEdit
                if (tcolumn_type == "tSearchEdit")
                {
                    tProp_Navigator = "Type:" + v.SearchEngine + ";" + tProp_Navigator;

                    // özel durum
                    if (tTableName.IndexOf("SYS_VARIABLES") > -1)
                        tProp_Navigator = "Type:" + v.SearchEngine + ";" + tTableName;
                    //---

                    tEdit.AccessibleDescription = tProp_Navigator;
                    //tEdit.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis;
                                        
                    DevExpress.XtraEditors.Controls.EditorButton tBtn =
                       new DevExpress.XtraEditors.Controls.EditorButton();
                    tBtn.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Search;
                                        
                    tEdit.Buttons.Add(tBtn);
                    
                }
                #endregion tSearchEdit
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region CalcEdit
            if (tcolumn_type == "CalcEdit")
            {
                RepositoryItemCalcEdit tEdit = new RepositoryItemCalcEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.AccessibleDescription = tProp_Navigator;

                if (tExpressionType > 0)
                    Column.Tag = "EXPRESSION";
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region CheckButton
            if (tcolumn_type == "CheckButton")
            {
            }
            #endregion

            #region CheckedComboBoxEdit
            if (tcolumn_type == "CheckedComboBoxEdit")
            {
                RepositoryItemCheckedComboBoxEdit tEdit = new RepositoryItemCheckedComboBoxEdit();
                tEdit.Name = "Column_" + tFieldName;

                RepositoryItemCheckedComboBoxEdit_Fill(tEdit, Row);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region CheckEdit
            if (tcolumn_type == "CheckEdit")
            {
                RepositoryItemCheckEdit tEdit = new RepositoryItemCheckEdit();
                tEdit.Name = "Column_" + tFieldName;

                short ftype = t.Set(Row["LKP_FIELD_TYPE"].ToString(), "", (short)0);

                if (ftype == 104) // bit
                {
                    tEdit.BeginInit();
                    tEdit.ValueChecked = true;
                    tEdit.ValueUnchecked = false;
                    tEdit.EndInit();
                }

                if (ftype == 52) // Int16
                {
                    tEdit.BeginInit();
                    tEdit.ValueChecked = ((Int16)(1));
                    tEdit.ValueUnchecked = ((Int16)(0));
                    tEdit.EndInit();
                }

                if (ftype == 56) // int
                {
                    tEdit.BeginInit();
                    tEdit.ValueChecked = (int)1;
                    tEdit.ValueUnchecked = (int)0;
                    tEdit.EndInit();
                }

                if (ftype == 167) // varchar
                {
                    tEdit.BeginInit();
                    tEdit.ValueUnchecked = "";
                    tEdit.ValueChecked = "E";
                    tEdit.ValueUnchecked = "H";
                    tEdit.EndInit();
                }

                if (tExpressionType > 0)
                    Column.Tag = "EXPRESSION";
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region CheckedListBoxControl
            if (tcolumn_type == "CheckedListBoxControl")
            {
            }
            #endregion

            #region ComboBoxEdit
            if (tcolumn_type == "ComboBoxEdit")
            {
                RepositoryItemComboBox tEdit = new RepositoryItemComboBox();
                tEdit.Name = "Column_" + tFieldName;

                RepositoryItemComboBox_Fill(tEdit, Row, "", 1); // Tumu = hayır
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region DateEdit
            if (tcolumn_type == "DateEdit")
            {
                RepositoryItemDateEdit tEdit = new RepositoryItemDateEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (tExpressionType > 0)
                    Column.Tag = "EXPRESSION";
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region HyperLinkEdit
            if (tcolumn_type == "HyperLinkEdit")
            {
                RepositoryItemHyperLinkEdit tEdit = new RepositoryItemHyperLinkEdit();
                tEdit.Name = "Column_" + tFieldName;
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region ImageComboBoxEdit
            if ((tcolumn_type == "ImageComboBoxEdit") ||
                (tcolumn_type == "tImageComboBoxEdit2Button") ||
                (tcolumn_type == "tImageComboBoxEditSEC") ||
                (tcolumn_type == "tImageComboBoxEditSubView")
                ) 
            {
                RepositoryItemImageComboBox tEdit = new RepositoryItemImageComboBox();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.AccessibleName = TableIPCode;
                //tEdit.Tag = field_no;

                RepositoryItemImageComboBox_Fill(tEdit, Row, "", 1); // Tumu = Hayır

                #region
                /*
                if (Row["LKP_TLKP_TYPE"] != null)
                    i = Convert.ToInt16(t.IsNull(Row["LKP_TLKP_TYPE"].ToString(), "0"));

                // TLKP_TYPE, FLKP_TYPE 

                // 103 = Kullanıcı Lkp ise
                if (i == 103)
                {
                    DevExpress.XtraEditors.Controls.EditorButton tBtn =
                        new DevExpress.XtraEditors.Controls.EditorButton();

                    // Esas Master olan field name
                    tBtn.Caption = Column.FieldName;

                    tEdit.Buttons.Add(tBtn);
       //             tEdit.ButtonClick += new
       //              DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(DevExpCC.ImageComboBoxEdit_ButtonClick);

       //             tEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(DevExpCC.ImageComboBoxEdit_KeyDown);
       //             tEdit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(DevExpCC.ImageComboBoxEdit_KeyPress);
       //             tEdit.KeyUp += new System.Windows.Forms.KeyEventHandler(DevExpCC.ImageComboBoxEdit_KeyUp);
                }

                // type : 101 = Proje LKP
                // type : 103 = Kullanıcı LKP
                // type : 105 = Data LKP  (HESAP PLANLARI)

                if (Row["LKP_LOOKUP_CODE"].ToString() != "")
                    List_Name = Row["LKP_LOOKUP_CODE"].ToString();

                //????????? unutma
                tEdit.Name = "tImageComboBoxEdit_" + List_Name;

                */
                #endregion
                
                if (tcolumn_type == "tImageComboBoxEdit2Button")
                {
                    DevExpress.XtraEditors.Controls.EditorButton tBtn =
                    new DevExpress.XtraEditors.Controls.EditorButton();

                    // Esas Master olan field name
                    tBtn.Caption = Column.FieldName;

                    tEdit.Buttons.Add(tBtn);
                    tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.ImageComboBoxEdit_ButtonClick);
                }

                if (tcolumn_type == "tImageComboBoxEditSEC")
                {
                    DevExpress.XtraEditors.Controls.EditorButton tBtn =
                       new DevExpress.XtraEditors.Controls.EditorButton();

                    tBtn.Caption = "SEÇ";
                    tBtn.Width = 30;
                    tBtn.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Search;

                    tProp_Navigator = "Type:" + v.SearchEngine + ";" + tProp_Navigator;
                    tEdit.AccessibleDescription = tProp_Navigator;

                    tEdit.ReadOnly = true;
                    tEdit.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
                    tEdit.Buttons[0].Visible = false;
                    tEdit.Buttons.Add(tBtn);

                    //tEdit.ButtonClick += new
                    //  DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.ImageComboBoxEdit_ButtonClick);
                    tEdit.ButtonClick += new
                      DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);
                }

                if  (tcolumn_type == "tImageComboBoxEditSubView")
                {
                    // grid
                }

                if (tExpressionType > 0)
                    Column.Tag = "EXPRESSION";

                Column.ColumnEdit = tEdit;
            }
            #endregion
            
            #region ImageEdit
            if (tcolumn_type == "ImageEdit")
            {
                RepositoryItemImageEdit tEdit = new RepositoryItemImageEdit();
                tEdit.Name = "Column_" + tFieldName;
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region ImageListBoxControl
            if (tcolumn_type == "ImageListBoxControl")
            {
                RepositoryItemImageComboBox tEdit = new RepositoryItemImageComboBox();
                tEdit.Name = "Column_" + tFieldName;
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region LabelControl
            if (tcolumn_type == "LabelControl")
            {
            }
            #endregion

            #region ListBoxControl
            if (tcolumn_type == "ListBoxControl")
            {

            }
            #endregion

            #region LookUpEdit
            if (tcolumn_type == "LookUpEdit")
            {
                RepositoryItemLookUpEdit tEdit = new RepositoryItemLookUpEdit();
                tEdit.Name = "Column_" + tFieldName;
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region MemoEdit
            if (tcolumn_type == "MemoEdit")
            {
                RepositoryItemMemoEdit tEdit = new RepositoryItemMemoEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.AutoHeight = true;
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region MemoExEdit
            if (tcolumn_type == "MemoExEdit")
            {
                RepositoryItemMemoExEdit tEdit = new RepositoryItemMemoExEdit();
                tEdit.Name = "Column_" + tFieldName;
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region MRUEdit
            if (tcolumn_type == "MRUEdit")
            {
                RepositoryItemMRUEdit tEdit = new RepositoryItemMRUEdit();
                tEdit.Name = "Column_" + tFieldName;
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region PictureEdit
            if (tcolumn_type == "PictureEdit")
            {
                RepositoryItemPictureEdit tEdit = new RepositoryItemPictureEdit();
                tEdit.Name = "Column_" + tFieldName;
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region PopupContainerControl
            if (tcolumn_type == "PopupContainerControl")
            {
            }
            #endregion

            #region PopupContainerEdit
            if (tcolumn_type == "PopupContainerEdit")
            {
            }
            #endregion

            #region RadioGroup
            if (tcolumn_type == "RadioGroup")
            {
                RepositoryItemRadioGroup tEdit = new RepositoryItemRadioGroup();
                tEdit.Name = "Column_" + tFieldName;

                RepositoryItemRadioGroup_Fill(tEdit, Row, "", 1); // Tumu = hayır
                Column.ColumnEdit = tEdit;
            }
            #endregion
            
            #region RangeTrackBarControl
            if (tcolumn_type == "RangeTrackBarControl")
            {
            }
            #endregion

            #region SpinEdit
            if (tcolumn_type == "SpinEdit")
            {
                RepositoryItemSpinEdit tEdit = new RepositoryItemSpinEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (tExpressionType > 0)
                    Column.Tag = "EXPRESSION";
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region TextEdit
            if (tcolumn_type == "TextEdit")
            {
                RepositoryItemTextEdit tEdit = new RepositoryItemTextEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.AccessibleName = TableIPCode;
                
                //if (tProp_Navigator != "")
                //{
                //    tProp_Navigator = "Type:" + v.SearchEngine + ";" + tProp_Navigator;

                //    tEdit.AccessibleDescription = tProp_Navigator;
                //    tEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myRepositoryItemEdit_KeyDown);
                //    //tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                //}

                #region displayformat
                if (t.IsNotNull(tdisplayformat))
                {
                    // 0,  'none'
                    // 1,  'Numeric'
                    // 2,  'DateTime'
                    // 3,  'Custom'

                    // a nın değeri sıfır döndüğü için b devreye giremiyor 
                    //string a = Row["CMP_FORMAT_TYPE"].ToString();
                    //string b = Row["LKP_CMP_FORMAT_TYPE"].ToString();

                    Int16 tcmp_format_type = t.Set(Row["CMP_FORMAT_TYPE"].ToString(), Row["LKP_CMP_FORMAT_TYPE"].ToString(), (Int16)0);
                    
                    RepositoryItem_DisplayFormat(tEdit, tdisplayformat, tcmp_format_type);
                }
                #endregion displayformat

                if (tExpressionType > 0)
                    Column.Tag = "EXPRESSION";
                
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region TimeEdit
            if (tcolumn_type == "TimeEdit")
            {
                RepositoryItemTimeEdit tEdit = new RepositoryItemTimeEdit();
                tEdit.Name = "Column_" + tFieldName;
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region TimeSpanEdit
            if (tcolumn_type == "TimeSpanEdit")
            {
                RepositoryItemTimeSpanEdit tEdit = new RepositoryItemTimeSpanEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.AllowEditDays = false;

                Column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                Column.ColumnEdit = tEdit;
            }
            #endregion
            
            #region TrackBarControl
            if (tcolumn_type == "TrackBarControl")
            {
                RepositoryItemTrackBar tEdit = new RepositoryItemTrackBar();
                tEdit.Name = "Column_" + tFieldName;
                if (tExpressionType > 0)
                    Column.Tag = "EXPRESSION";
                Column.ColumnEdit = tEdit;
            }
            #endregion
            
            /// gridView1.Columns["Column2"].AppearanceHeader.Font = new Font("Times New Roman", 15);
            /// gridView1.Columns["Column2"].AppearanceCell.Font = new Font("Times New Roman", 15);
            /// 
            
            #region Column Properties Changes
            if ((tcolumn_type != "tPropertiesNavEdit") &&
                (tcolumn_type != "tPropertiesEdit") &&
                (tcolumn_type != "tPropertiesPlusEdit"))
            {
                float font_size = 0;
                int vld_operator = 0;
                Int16 font_style = 0;
                Boolean tenabled = true;

                font_size = t.Set(Row["CMP_FONT_SIZE"].ToString(), "", (float)0);
                font_style = t.Set(Row["CMP_FONT_STYLE"].ToString(), "", (Int16)0);
                tenabled = t.Set(Row["CMP_ENABLED"].ToString(), Row["LKP_FENABLED"].ToString(), true);
                vld_operator = t.Set(Row["VALIDATION_OPERATOR"].ToString(), Row["LKP_VALIDATION_OPERATOR"].ToString(), (int)150);

                /// 'FONT_STYLE', 0, '', 'None'
                /// 'FONT_STYLE', 1, '', 'Bold');
                /// 'FONT_STYLE', 2, '', 'Italic');
                /// 'FONT_STYLE', 3, '', 'Regular');
                /// 'FONT_STYLE', 4, '', 'Strikeout');
                /// 'FONT_STYLE', 5, '', 'Underline');

                if (font_size > 0)
                {
                    if ((font_size != 8) || (font_style != 0))
                    {
                        if (font_style == 0) Column.AppearanceCell.Font = new System.Drawing.Font(Column.AppearanceCell.Font.FontFamily, font_size, FontStyle.Regular);
                        if (font_style == 1) Column.AppearanceCell.Font = new System.Drawing.Font(Column.AppearanceCell.Font.FontFamily, font_size, FontStyle.Bold);
                        if (font_style == 2) Column.AppearanceCell.Font = new System.Drawing.Font(Column.AppearanceCell.Font.FontFamily, font_size, FontStyle.Italic);
                        if (font_style == 3) Column.AppearanceCell.Font = new System.Drawing.Font(Column.AppearanceCell.Font.FontFamily, font_size, FontStyle.Regular);
                        if (font_style == 4) Column.AppearanceCell.Font = new System.Drawing.Font(Column.AppearanceCell.Font.FontFamily, font_size, FontStyle.Strikeout);
                        if (font_style == 5) Column.AppearanceCell.Font = new System.Drawing.Font(Column.AppearanceCell.Font.FontFamily, font_size, FontStyle.Underline);
                        Column.AppearanceCell.Options.UseFont = true;
                    }
                }
                               

                if (vld_operator > 11)
                {
                    Column.AppearanceCell.BackColor = v.Validate_New;
                    Column.AppearanceCell.Options.UseBackColor = true;
                }

                Column.OptionsColumn.AllowEdit = tenabled;
            }
            #endregion Column Properties Changes

        }
                
        #endregion
        
        #region VGrid_ColumnEdit

        public void VGrid_ColumnEdit(DataRow Row, EditorRow Column, string tcolumn_type)
        {
            tToolBox t = new tToolBox();

            string s = string.Empty;
            string TableCode = string.Empty;
            string IPCode = string.Empty;
            string TableIPCode = string.Empty;
            string tTableName = string.Empty;
            string tFieldName = string.Empty;
            string ttype_name = string.Empty;
            string tfieldtype = string.Empty;
            string tdisplayformat = string.Empty;
            string tcmp_format_type = string.Empty;
            string tProp_Navigator = string.Empty;

            Int16 width = 0;
                            
            if (Row != null)
            {
                TableCode = Row["TABLE_CODE"].ToString();
                IPCode = Row["IP_CODE"].ToString();
                TableIPCode = TableCode + "." + IPCode; 
                tTableName = Row["LKP_TABLE_NAME"].ToString();
                tFieldName = Row["LKP_FIELD_NAME"].ToString();
                ttype_name = t.Set(Row["LIST_TYPES_NAME"].ToString(), Row["LKP_LIST_TYPES_NAME"].ToString(), "");
                width = t.Set(Row["CMP_WIDTH"].ToString(), Row["LKP_CMP_WIDTH"].ToString(), (Int16)200);
                tfieldtype = t.Set(Row["LKP_FIELD_TYPE"].ToString(), "", "0");
                tdisplayformat = t.Set(Row["CMP_DISPLAY_FORMAT"].ToString(), Row["LKP_CMP_DISPLAY_FORMAT"].ToString(), "");
                tcmp_format_type = t.Set(Row["CMP_FORMAT_TYPE"].ToString(), Row["LKP_CMP_FORMAT_TYPE"].ToString(), "0");
                tProp_Navigator = t.Set(Row["PROP_NAVIGATOR"].ToString(), "", "");

                t.MyProperties_Set(ref s, "TableIPCode", TableIPCode);
                t.MyProperties_Set(ref s, "TableName", tTableName);
                t.MyProperties_Set(ref s, "FieldName", tFieldName);
                t.MyProperties_Set(ref s, "ColumnType", tcolumn_type);
                t.MyProperties_Set(ref s, "TypeName", ttype_name);
                t.MyProperties_Set(ref s, "FieldType", tfieldtype);
                t.MyProperties_Set(ref s, "Width", width.ToString());
                t.MyProperties_Set(ref s, "DisplayFormat", tdisplayformat);
                t.MyProperties_Set(ref s, "CmpFormatType", tcmp_format_type);
                //t.MyProperties_Set(ref s, "Prop_Search", tProp_Navigator);
            }
                        
            if (t.IsNotNull(s))
                VGrid_ColumnEdit_(Row, Column, s, tProp_Navigator, 1); // Tumu = hayır
        }

        public void VGrid_ColumnEdit_(DataRow row_Fields, EditorRow Column, string myProp, string tProp_Navigator, byte tview_type)
        {
            tToolBox t = new tToolBox();
            tEvents ev = new tEvents();

            #region Tanımlar
            string TableIPCode = t.MyProperties_Get(myProp, "TableIPCode:");
            string tTableName = t.MyProperties_Get(myProp, "TableName:");
            string tFieldName = t.MyProperties_Get(myProp, "FieldName:");
            string tcolumn_type = t.MyProperties_Get(myProp, "ColumnType:");
            string ttype_name = t.MyProperties_Get(myProp, "TypeName:");
            string tfieldtype = t.Set(t.MyProperties_Get(myProp, "FieldType:"),"","0");
            string twidth = t.MyProperties_Get(myProp, "Width:");
            string tdisplayformat = t.MyProperties_Get(myProp, "DisplayFormat:");
            Int16  tcmp_format_type = t.Set(t.MyProperties_Get(myProp, "CmpFormatType:"), "0", (Int16)0);
            #endregion Tanımlar
                        

            #region tPropertiesNavEdit
            if (tcolumn_type == "tPropertiesNavEdit")
            {
                RepositoryItemButtonEdit tEdit = new RepositoryItemButtonEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                tEdit.KeyDown += new KeyEventHandler(ev.buttonEdit_KeyDown);
                tEdit.KeyPress += new KeyPressEventHandler(ev.buttonEdit_KeyPress);
                tEdit.KeyUp += new KeyEventHandler(ev.buttonEdit_KeyUp);
                tEdit.Leave += new EventHandler(ev.buttonEdit_Leave);
                tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);

                //Int16 width = t.Set(Row["LKP_CMP_WIDTH"].ToString(), "", (Int16)200);

                string s = string.Empty;
                t.MyProperties_Set(ref s, "Type", v.PropertiesNav);
                //t.MyProperties_Set(ref s, "TableName", tTableName);
                //t.MyProperties_Set(ref s, "FieldName", tFieldName);
                //t.MyProperties_Set(ref s, "Width", width.ToString());

                tEdit.AccessibleDescription = s;
                //if (font_size != 0)
                //    tEdit.Appearance.Font = new Font("Tahoma", font_size);

                Column.Properties.RowEdit = tEdit;
            }
            #endregion
            
            #region tPropertiesEdit
            if (tcolumn_type == "tPropertiesEdit")
            {
                RepositoryItemButtonEdit tEdit = new RepositoryItemButtonEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                tEdit.KeyDown += new KeyEventHandler(ev.buttonEdit_KeyDown);
                tEdit.KeyPress += new KeyPressEventHandler(ev.buttonEdit_KeyPress);
                tEdit.KeyUp += new KeyEventHandler(ev.buttonEdit_KeyUp);
                tEdit.Leave += new EventHandler(ev.buttonEdit_Leave);
                tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);

                
                string s = string.Empty;
                t.MyProperties_Set(ref s, "Type", v.Properties);
                t.MyProperties_Set(ref s, "TableName", tTableName);
                t.MyProperties_Set(ref s, "FieldName", tFieldName);
                t.MyProperties_Set(ref s, "Width", twidth);

                tEdit.AccessibleDescription = s; 

                Column.Properties.RowEdit = tEdit;
            }
            #endregion

            #region tPropertiesPlusEdit
            if (tcolumn_type == "tPropertiesPlusEdit")
            {
                RepositoryItemButtonEdit tEdit = new RepositoryItemButtonEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                tEdit.KeyDown += new KeyEventHandler(ev.buttonEdit_KeyDown);
                tEdit.KeyPress += new KeyPressEventHandler(ev.buttonEdit_KeyPress);
                tEdit.KeyUp += new KeyEventHandler(ev.buttonEdit_KeyUp);
                tEdit.Leave += new EventHandler(ev.buttonEdit_Leave);
                tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);
                
                string s = string.Empty;
                t.MyProperties_Set(ref s, "Type", v.PropertiesPlus);
                t.MyProperties_Set(ref s, "TableName", tTableName);
                t.MyProperties_Set(ref s, "FieldName", tFieldName);
                t.MyProperties_Set(ref s, "Width", twidth);

                tEdit.AccessibleDescription = s; 

                Column.Properties.RowEdit = tEdit;
            }
            #endregion

            #region ButtonEdit / tSearchEdit
            if ((tcolumn_type == "ButtonEdit") ||
                (tcolumn_type == "tSearchEdit"))
            {
                RepositoryItemButtonEdit tEdit = new RepositoryItemButtonEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.AccessibleName = TableIPCode;
                tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                tEdit.KeyDown += new KeyEventHandler(ev.buttonEdit_KeyDown);
                tEdit.KeyPress += new KeyPressEventHandler(ev.buttonEdit_KeyPress);
                tEdit.KeyUp += new KeyEventHandler(ev.buttonEdit_KeyUp);
                tEdit.Leave += new EventHandler(ev.buttonEdit_Leave);
                tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);

                tEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                if (tFieldName == "LKP_KOMUT")
                {
                    tEdit.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.OK;
                    tEdit.Buttons[0].Width = 40;// tEdit.BestFitWidth / 2;
                    tEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                }

                #region tSearchEdit
                if (tcolumn_type == "tSearchEdit")
                {
                    tProp_Navigator = "Type:" + v.SearchEngine + ";" + tProp_Navigator;
                    tEdit.AccessibleDescription = tProp_Navigator;
                    
                    DevExpress.XtraEditors.Controls.EditorButton tBtn =
                       new DevExpress.XtraEditors.Controls.EditorButton();
                    tBtn.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Search;

                    tEdit.Buttons.Add(tBtn);
                    
                }
                #endregion tSearchEdit

                Column.Properties.RowEdit = tEdit;
            }
            #endregion
            
            #region CalcEdit
            if (tcolumn_type == "CalcEdit")
            {
                RepositoryItemCalcEdit tCalcEdit = new RepositoryItemCalcEdit();

                tCalcEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tCalcEdit;
            }
            #endregion

            #region CheckButton
            if (tcolumn_type == "CheckButton")
            {
            }
            #endregion

            #region CheckedComboBoxEdit
            if (tcolumn_type == "CheckedComboBoxEdit")
            {
                RepositoryItemCheckedComboBoxEdit tEdit = new RepositoryItemCheckedComboBoxEdit();

                string List_Name = ttype_name;

                if (t.IsNotNull(List_Name))
                {
                    RepositoryItemCheckedComboBoxEdit_Fill_(tEdit, null, List_Name);
                }
                
                tEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tEdit;
            }
            #endregion

            #region CheckEdit
            if (tcolumn_type == "CheckEdit")
            {
                RepositoryItemCheckEdit tEdit = new RepositoryItemCheckEdit();

                byte ftype = Convert.ToByte(tfieldtype.ToString());

                if (ftype == 52) // Int16
                {
                    tEdit.BeginInit();
                    tEdit.ValueChecked = ((short)(1));
                    tEdit.ValueUnchecked = ((short)(0));
                    tEdit.EndInit();
                }

                if (ftype == 56) // int
                {
                    tEdit.BeginInit();
                    tEdit.ValueChecked = 1;
                    tEdit.ValueUnchecked = 0;
                    tEdit.EndInit();
                }

                if (ftype == 167) // varchar
                {
                    tEdit.BeginInit();
                    tEdit.ValueUnchecked = "";
                    tEdit.ValueChecked = "E";
                    tEdit.ValueUnchecked = "H";
                    tEdit.EndInit();
                }

                tEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tEdit;
            }
            #endregion

            #region CheckedListBoxControl
            if (tcolumn_type == "CheckedListBoxControl")
            {
            }
            #endregion

            #region ComboBoxEdit
            if (tcolumn_type == "ComboBoxEdit")
            {
                RepositoryItemComboBox tEdit = new RepositoryItemComboBox();

                string List_Name = ttype_name;

                if (t.IsNotNull(List_Name))
                {
                    tRepositoryItem_Fill(null, null, tEdit, null, null, null, List_Name, "", 1); // Tumu Hayır
                }
                
                tEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tEdit;
            }
            #endregion

            #region DateEdit
            if (tcolumn_type == "DateEdit")
            {
                RepositoryItemDateEdit tDateEdit = new RepositoryItemDateEdit();

                tDateEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tDateEdit;
            }
            #endregion

            #region HyperLinkEdit
            if (tcolumn_type == "HyperLinkEdit")
            {
                RepositoryItemHyperLinkEdit tHyperLinkEdit = new RepositoryItemHyperLinkEdit();

                tHyperLinkEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tHyperLinkEdit;
            }
            #endregion
            
            #region ImageComboBoxEdit
            if ((tcolumn_type == "ImageComboBoxEdit") ||
                (tcolumn_type == "tImageComboBoxEdit2Button") ||
                (tcolumn_type == "tImageComboBoxEditSEC") ||
                (tcolumn_type == "tImageComboBoxEditSubView")
                )
            {
                RepositoryItemImageComboBox tEdit = new RepositoryItemImageComboBox();
                tEdit.Name = "Column_" + tFieldName;

                string List_Name = ttype_name;
                
                if (t.IsNotNull(List_Name))
                {
                    tRepositoryItem_Fill(tEdit, null, null, null, null, null, List_Name, "", tview_type);
                }

                if (tcolumn_type == "tImageComboBoxEdit2Button")
                {
                    DevExpress.XtraEditors.Controls.EditorButton tBtn =
                    new DevExpress.XtraEditors.Controls.EditorButton();

                    // Esas Master olan field name
                    tBtn.Caption = Column.Properties.FieldName;

                    tEdit.Buttons.Add(tBtn);
                    tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.ImageComboBoxEdit_ButtonClick);
                }

                if (tcolumn_type == "tImageComboBoxEditSEC")
                {
                    DevExpress.XtraEditors.Controls.EditorButton tBtn =
                       new DevExpress.XtraEditors.Controls.EditorButton();

                    tBtn.Caption = "SEÇ";
                    tBtn.Width = 30;
                    tBtn.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Search;

                    tProp_Navigator = "Type:" + v.SearchEngine + ";" + tProp_Navigator;
                    tEdit.AccessibleDescription = tProp_Navigator;

                    tEdit.ReadOnly = true;
                    tEdit.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
                    tEdit.Buttons[0].Visible = false;
                    tEdit.Buttons.Add(tBtn);

                    //tEdit.ButtonClick += new
                    //  DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.ImageComboBoxEdit_ButtonClick);
                    tEdit.ButtonClick += new
                      DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);
                }

                if (tcolumn_type == "tImageComboBoxEditSubView")
                {
                    // Vgrid
                }
                
                Column.Properties.RowEdit = tEdit;
            }
            #endregion
       
            #region ImageEdit
            if (tcolumn_type == "ImageEdit")
            {
                RepositoryItemImageEdit tImageEdit = new RepositoryItemImageEdit();

                tImageEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tImageEdit;
            }
            #endregion

            #region ImageListBoxControl
            if (tcolumn_type == "ImageListBoxControl")
            {
                RepositoryItemImageComboBox tImageListBoxControl = new RepositoryItemImageComboBox();

                tImageListBoxControl.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tImageListBoxControl;
            }
            #endregion

            #region LabelControl
            if (tcolumn_type == "LabelControl")
            {
            }
            #endregion

            #region ListBoxControl
            if (tcolumn_type == "ListBoxControl")
            {

            }
            #endregion

            #region LookUpEdit
            if (tcolumn_type == "LookUpEdit")
            {
                RepositoryItemLookUpEdit tLookUpEdit = new RepositoryItemLookUpEdit();

                tLookUpEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tLookUpEdit;
            }
            #endregion

            #region MemoEdit
            if (tcolumn_type == "MemoEdit")
            {
                RepositoryItemMemoEdit tMemoEdit = new RepositoryItemMemoEdit();

                tMemoEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tMemoEdit;
            }
            #endregion

            #region MemoExEdit
            if (tcolumn_type == "MemoExEdit")
            {
                RepositoryItemMemoExEdit tMemoExEdit = new RepositoryItemMemoExEdit();

                tMemoExEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tMemoExEdit;
            }
            #endregion

            #region MRUEdit
            if (tcolumn_type == "MRUEdit")
            {
                RepositoryItemMRUEdit tMRUEdit = new RepositoryItemMRUEdit();

                tMRUEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tMRUEdit;
            }
            #endregion

            #region PictureEdit
            if (tcolumn_type == "PictureEdit")
            {
                RepositoryItemPictureEdit tPictureEdit = new RepositoryItemPictureEdit();

                tPictureEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tPictureEdit;
            }
            #endregion

            #region PopupContainerControl
            if (tcolumn_type == "PopupContainerControl")
            {
            }
            #endregion

            #region PopupContainerEdit
            if (tcolumn_type == "PopupContainerEdit")
            {
            }
            #endregion

            #region RadioGroup
            if (tcolumn_type == "RadioGroup")
            {
                RepositoryItemRadioGroup tEdit = new RepositoryItemRadioGroup();
                tEdit.Name = "Column_" + tFieldName;

                string List_Name = ttype_name;

                tRepositoryItem_Fill(null, null, null, null, tEdit, null, List_Name, "", 1); // Tumu hayır

                Column.Properties.RowEdit = tEdit;
            }
            #endregion
            
            #region RangeTrackBarControl
            if (tcolumn_type == "RangeTrackBarControl")
            {
            }
            #endregion

            #region SpinEdit
            if (tcolumn_type == "SpinEdit")
            {
                RepositoryItemSpinEdit tSpinEdit = new RepositoryItemSpinEdit();

                tSpinEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tSpinEdit;
            }
            #endregion

            #region TextEdit
            if (tcolumn_type == "TextEdit")
            {
                //RepositoryItemTextEdit tTextEdit = new RepositoryItemTextEdit();
                RepositoryItemTextEdit tEdit = new RepositoryItemTextEdit();

                tEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);
                
                RepositoryItem_DisplayFormat(tEdit, tdisplayformat, tcmp_format_type);

                Column.Properties.RowEdit = tEdit;
            }
            #endregion

            #region TimeEdit
            if (tcolumn_type == "TimeEdit")
            {
                RepositoryItemTimeEdit tTimeEdit = new RepositoryItemTimeEdit();

                tTimeEdit.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                tTimeEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;

                //tTimeEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tTimeEdit;
            }
            #endregion

            #region TimeSpanEdit
            if (tcolumn_type == "TimeSpanEdit")
            {
                RepositoryItemTimeSpanEdit tEdit = new RepositoryItemTimeSpanEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.AllowEditDays = false;

                //if (font_size != 0)
                //    tEdit.Appearance.Font = new Font("Tahoma", font_size);

                Column.Properties.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                Column.Properties.RowEdit = tEdit;
            }
            #endregion

            #region TrackBarControl
            if (tcolumn_type == "TrackBarControl")
            {
                RepositoryItemTrackBar tTrackBarControl = new RepositoryItemTrackBar();

                tTrackBarControl.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myVGridControl_Kriter_KeyDown);

                Column.Properties.RowEdit = tTrackBarControl;
            }
            #endregion


            #region Column Properties Changes

            if ((tcolumn_type != "tPropertiesNavEdit") &&
                (tcolumn_type != "tPropertiesEdit") &&
                (tcolumn_type != "tPropertiesPlusEdit") &&
                (tfieldtype != "0"))// tProperties içinden ImageComboBoxEdit geliyor
            {
                float font_size = 0;
                int vld_operator = 0;
                Int16 font_style = 0;
                Boolean tenabled = true;

                font_size = t.Set(row_Fields["CMP_FONT_SIZE"].ToString(), "", (float)0);
                font_style = t.Set(row_Fields["CMP_FONT_STYLE"].ToString(), "", (Int16)0);
                tenabled = t.Set(row_Fields["CMP_ENABLED"].ToString(), row_Fields["LKP_FENABLED"].ToString(), true);
                vld_operator = t.Set(row_Fields["VALIDATION_OPERATOR"].ToString(), row_Fields["LKP_VALIDATION_OPERATOR"].ToString(), (int)150);

                /// 'FONT_STYLE', 0, '', 'None'
                /// 'FONT_STYLE', 1, '', 'Bold');
                /// 'FONT_STYLE', 2, '', 'Italic');
                /// 'FONT_STYLE', 3, '', 'Regular');
                /// 'FONT_STYLE', 4, '', 'Strikeout');
                /// 'FONT_STYLE', 5, '', 'Underline');

                if (font_size > 0)
                {
                    if (font_size != 8)
                    {
                        Column.Appearance.Font = new System.Drawing.Font(Column.Appearance.Font.FontFamily, font_size);
                        Column.Appearance.Options.UseFont = true;
                    }
                }

                if (font_style > 0)
                {
                    if (font_style == 1)
                        Column.Appearance.Font = new System.Drawing.Font(Column.Appearance.Font.FontFamily, Column.Appearance.Font.Size, FontStyle.Bold);
                    if (font_style == 2)
                        Column.Appearance.Font = new System.Drawing.Font(Column.Appearance.Font.FontFamily, Column.Appearance.Font.Size, FontStyle.Italic);
                    Column.Appearance.Options.UseFont = true;
                }

                if (vld_operator > 11)
                {
                    Column.Appearance.BackColor = v.Validate_New;
                    Column.Appearance.Options.UseBackColor = true;
                }

                Column.Enabled = tenabled;
            }
            #endregion Column Properties Changes
        }
                
        #endregion VGrid_ColumnEdit

        #region tXtraEditors Edit

        public void tXtraEditors_tPictureEdit(DataRow row_Fields, DataSet dsData, Object Column, DataLayoutControl tDLayout)
        {
            //tToolBox t = new tToolBox();
            //tEvents ev = new tEvents();
            //tCreateObject co = new tCreateObject();

            //#region Tanımlar
            //string fname = string.Empty;
            //Int16 group_no = 0;
            //Boolean tvisible = false;
            //Boolean tTabPage = false;
            //Int16 group_types = 0;
            //Int16 fgroup_no = 0;
            //Int16 fgroup_line_no = 0;
            
            //#endregion

            //DevExpress.XtraLayout.BaseLayoutItem bl_item1 = null;

            ////DataSet dsFields, 

            //tvisible = t.Set(row_Fields["CMP_VISIBLE"].ToString(), row_Fields["LKP_FVISIBLE"].ToString(), true);
            //fgroup_no = t.Set(row_Fields["GROUP_NO"].ToString(), row_Fields["LKP_GROUP_NO"].ToString(), (Int16)0);
            //fgroup_line_no = t.Set(row_Fields["GROUP_LINE_NO"].ToString(), row_Fields["LKP_GROUP_LINE_NO"].ToString(), (Int16)0); ;

            //if (fgroup_no == -1)
            //    fgroup_no = t.Set(row_Fields["LKP_GROUP_NO"].ToString(), "0", (Int16)0);
            //if (fgroup_line_no == -1)
            //    fgroup_line_no = t.Set(row_Fields["LKP_GROUP_LINE_NO"].ToString(), "0", (Int16)0); ;

            
            

            //#region // Açılan Group veya TabPage döngüsü ve Move İşlemi
            //for (int i = 0; i < dsFields.Tables[1].Rows.Count; i++)
            //{
            //    /* 1 = GROUPCONTROL, 2 = TABPAGE CONTROL */
            //    group_types = t.Set(dsFields.Tables[1].Rows[i]["GROUP_TYPES"].ToString(), "", (Int16)0);
            //    group_no = t.Set(dsFields.Tables[1].Rows[i]["FGROUPNO"].ToString(), "", (Int16)0);

            //    if (fgroup_no > 0)
            //    {
            //        if ((group_no == fgroup_no) && (tvisible))
            //        {
            //            if (group_types == 1)
            //                fname = "LCGroup_" + group_no;
            //            if (group_types == 2)
            //                fname = "LCTabbedGroup_" + group_no;

            //            //if (tGridView != null)
            //            //    bl_item1 = tGridView.Items.FindByName(fname);

            //            if (tDLayout != null)
            //            {
            //                bl_item1 = tDLayout.Root.Items.FindByName(fname);

            //              //  co.Create_PictureControl(bl_item1, "", "");
            //            }
                        
            //            //fname = "Column_" + t.Set(dsFields.Tables[0].Rows[i]["LKP_FIELD_NAME"].ToString(), "", "");
            //            //bl_item1 = tDLayout.Root.Items.FindByName(fname);
            //            //LCGroup.Add(bl_item1);
            //        }
            //    }


            //}


        }

        public void tXtraEditors_Edit(DataRow row_Fields, DataSet dsData,
               DataLayoutControl tDLayout, Object Column,
               string tcolumn_type, string tdisplayformat, byte tview_type, string FormName)
        {
            tToolBox t = new tToolBox();
            tEvents ev = new tEvents();

            #region Tanımlar

            string function_name = "tXtraEditors Edit";
            string s = string.Empty;
            string TableIPCode = string.Empty;
            string default_value = string.Empty;
            string tTableName = string.Empty;
            string tFieldName = string.Empty;
            string tExpression = string.Empty;
            string tProp_Navigator = string.Empty;
            string tSearch_TableIPCode = string.Empty;

            Int16 tcmp_format_type = 0;
            Int16 tExpressionType = 0;
            int field_no = 0;

            #region row_Fields != null
            if (row_Fields != null)
            { 
                tTableName = row_Fields["LKP_TABLE_NAME"].ToString();
                tFieldName = row_Fields["LKP_FIELD_NAME"].ToString();
                tExpression = row_Fields["LKP_PROP_EXPRESSION"].ToString();
                tProp_Navigator = t.Set(row_Fields["PROP_NAVIGATOR"].ToString(), "", "");
                tSearch_TableIPCode = row_Fields["SEARCH_TABLEIPCODE"].ToString(); 

                /// 0,  'none'
                /// 1,  'Numeric'
                /// 2,  'DateTime'
                /// 3,  'Custom'
                tcmp_format_type = t.Set(row_Fields["CMP_FORMAT_TYPE"].ToString(), row_Fields["LKP_CMP_FORMAT_TYPE"].ToString(), (Int16)0);
                /// formülleri çalıştırmak için etkisi varmı
                /// 0 = yok
                /// 1 = true
                tExpressionType = t.myInt16(row_Fields["LKP_EXPRESSION_TYPE"].ToString());
                // 
                field_no = t.myInt32(row_Fields["FIELD_NO"].ToString());

                // FORMÜL varsa / gerek kalmadı
                if (t.IsNotNull(tExpression))
                {
                    //dsData.Tables[0].Columns[tFieldName]. //= tExpression;
                    //dsData.Tables[0].Columns[tFieldName].DataType = System.Type.GetType("System.Decimal");
                    //dsData.Tables[0].Columns[tFieldName].Expression = tExpression;
                }

                if (tview_type == 2) // Tumu varsa 
                {
                    byte default_type = t.Set(row_Fields["DEFAULT_TYPE"].ToString(), row_Fields["LKP_DEFAULT_TYPE"].ToString(), (byte)0);

                    // Tespit edilen Default Type Göre -------------------
                    #region default_type > Var ( 1, 2, 3, 4 )
                    if ((default_type >= 1) &&
                        (default_type <= 4))
                    {
                        if (default_type == 1) // Var (Integer)
                        {
                            default_value = t.Set(row_Fields["DEFAULT_INT"].ToString(), row_Fields["LKP_DEFAULT_INT"].ToString(), "0");
                        }

                        if (default_type == 2) // Var (Numeric)
                        {
                            default_value = t.Set(row_Fields["DEFAULT_NUMERIC"].ToString(), row_Fields["LKP_DEFAULT_NUMERIC"].ToString(), "0");
                        }

                        if (default_type == 3) // Var (Text)
                        {
                            default_value = t.Set(row_Fields["DEFAULT_TEXT"].ToString(), row_Fields["LKP_DEFAULT_TEXT"].ToString(), "");
                        }

                        if (default_type == 4) // Var (Standart) SP_xxxxx
                        {
                            tDefaultValue dv = new tDefaultValue();
                            s = t.Set(row_Fields["DEFAULT_SP"].ToString(), row_Fields["LKP_DEFAULT_SP"].ToString(), "0");
                            if (s != "0")
                                default_value = dv.tSP_Value_Load(s);
                        }
                    }
                    #endregion Var
                }

            }
            #endregion row_Fields != null

            #region Column.ToString()
            if (Column.ToString().IndexOf("System.Windows.Forms.Panel") > -1)
            {
                s = ""; //"System.Windows.Forms.Panel";
                if (((System.Windows.Forms.Panel)Column).AccessibleName != null)
                      TableIPCode = ((System.Windows.Forms.Panel)Column).AccessibleName.ToString();
            }

            if (Column.ToString() == "DevExpress.XtraLayout.LayoutControlItem")
            {
                s = "DevExpress.XtraLayout.LayoutControlItem";
                if (((LayoutControlItem)Column).ParentName != null)
                      TableIPCode = ((LayoutControlItem)Column).ParentName.ToString();
            }

            if (Column.ToString().IndexOf("DevExpress.XtraEditors.PanelControl") > -1)
            {
                s = "";// "DevExpress.XtraEditors.PanelControl";
                if (((DevExpress.XtraEditors.PanelControl)Column).AccessibleName != null)
                      TableIPCode = ((DevExpress.XtraEditors.PanelControl)Column).AccessibleName.ToString();
                if (((DevExpress.XtraEditors.PanelControl)Column).AccessibleDescription != null)
                      tFieldName = ((DevExpress.XtraEditors.PanelControl)Column).AccessibleDescription.ToString();
            }
            #endregion Column.ToString()

            if (TableIPCode == string.Empty)
            {
                MessageBox.Show(" TableIPCode belli değil " + v.ENTER + Column.ToString(), function_name);
                return;
            }
            #endregion 

            #region ButtonEdit // tSearchEdit
            if ((tcolumn_type == "ButtonEdit") ||
                (tcolumn_type == "tSearchEdit"))
            {
                ButtonEdit tEdit = new DevExpress.XtraEditors.ButtonEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                //tEdit.KeyDown += new KeyEventHandler(ev.buttonEdit_KeyDown);
                tEdit.KeyPress += new KeyPressEventHandler(ev.buttonEdit_KeyPress);
                tEdit.KeyUp += new KeyEventHandler(ev.buttonEdit_KeyUp);
                tEdit.Leave += new EventHandler(ev.buttonEdit_Leave);
                tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);
                tEdit.Tag = field_no;
                tEdit.Properties.AccessibleName = TableIPCode;
                
                if (tFieldName == "LKP_KOMUT")
                {
                    tEdit.Properties.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.OK;
                    tEdit.Properties.Buttons[0].Width = 40;// tEdit.Width / 2;
                    tEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                }

                #region ButtonEdit
                if (tcolumn_type == "ButtonEdit")
                {
                    tProp_Navigator = "Type:" + v.ButtonEdit + ";" + tProp_Navigator;
                    tEdit.Properties.AccessibleDescription = tProp_Navigator;

                }
                #endregion 

                #region tSearchEdit
                if (tcolumn_type == "tSearchEdit")
                {
                    tProp_Navigator = "Type:" + v.SearchEngine + ";" + tProp_Navigator;
                    tEdit.Properties.AccessibleDescription = tProp_Navigator;
                    tEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.myRepositoryItemEdit_KeyDown);
                    
                    DevExpress.XtraEditors.Controls.EditorButton tBtn =
                       new DevExpress.XtraEditors.Controls.EditorButton();
                    tBtn.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Search;

                    tEdit.Properties.Buttons.Add(tBtn);

                }
                #endregion tSearchEdit

                if (t.IsNotNull(tSearch_TableIPCode))
                {
                    tEdit.Properties.AccessibleDefaultActionDescription = tSearch_TableIPCode;
                    tEdit.EditValueChanged += new System.EventHandler(ev.textEdit_Find_EditValueChanged);
                    tEdit.Enter += new System.EventHandler(ev.textEdit_Find_Enter);
                    tEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.textEdit_Find_KeyDown);
                    tEdit.BackColor = Color.MintCream;
                    // burda geçici bir değer atadım
                    // ilk çalışmaya başladığında ise gerçek değerini alacak
                    // findType = 100 veya 200
                    tEdit.Tag = -100;
                }
                
                if (dsData != null)
                   tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));
                if (tDLayout != null)
                    tEdit.StyleController = tDLayout;

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                if (t.IsNotNull(tdisplayformat))
                    tEdit.Properties.DisplayFormat.FormatString = tdisplayformat;

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
                
            }
            #endregion

            #region CalcEdit
            if (tcolumn_type == "CalcEdit")
            {
                CalcEdit tEdit = new DevExpress.XtraEditors.CalcEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                //tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));
                if (tDLayout != null)
                    tEdit.StyleController = tDLayout;

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                #region displayformat
                if (t.IsNotNull(tdisplayformat))
                {
                    XtraEditorsTextEdit_DisplayFormat(tEdit, tdisplayformat, tcmp_format_type);
                }
                #endregion displayformat

                #region tExpressionType
                if (tExpressionType > 0)
                {
                    tEdit.Properties.Tag = "EXPRESSION";
                    tEdit.EditValueChanged += new System.EventHandler(ev.tXtraEdit_EditValueChanged);
                    tEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(ev.tXtraEdit_EditValueChanging);
                    tEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.tXtraEdit_KeyDown);
                    tEdit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(ev.tXtraEdit_KeyPress);
                    tEdit.KeyUp += new System.Windows.Forms.KeyEventHandler(ev.tXtraEdit_KeyUp);
                }
                #endregion 

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);

            }
            #endregion

            #region CheckButton
            if (tcolumn_type == "CheckButton")
            {
            }
            #endregion

            #region CheckedComboBoxEdit
            if (tcolumn_type == "CheckedComboBoxEdit")
            {
                CheckedComboBoxEdit tEdit = new DevExpress.XtraEditors.CheckedComboBoxEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));
                if (tDLayout != null)
                    tEdit.StyleController = tDLayout;

                RepositoryItemCheckedComboBoxEdit_Fill(tEdit, row_Fields);
                                
                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region CheckEdit
            if (tcolumn_type == "CheckEdit")
            {
                CheckEdit tEdit = new DevExpress.XtraEditors.CheckEdit();
                
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.Caption = "";
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));
                if (tDLayout != null)
                    tEdit.StyleController = tDLayout;

                byte ftype = t.Set(row_Fields["LKP_FIELD_TYPE"].ToString(), "", (byte)0);

                if (ftype == 52) // Int16
                {
                    tEdit.Properties.BeginInit();
                    tEdit.Properties.ValueChecked = ((short)(1));
                    tEdit.Properties.ValueUnchecked = ((short)(0));
                    tEdit.Properties.EndInit();
                }

                if (ftype == 56) // int
                {
                    tEdit.Properties.BeginInit();
                    tEdit.Properties.ValueChecked = 1;
                    tEdit.Properties.ValueUnchecked = 0;
                    tEdit.Properties.EndInit();
                }


                if (ftype == 167) // varchar
                {
                    tEdit.Properties.BeginInit();
                    tEdit.Properties.ValueUnchecked = "";
                    tEdit.Properties.ValueChecked = "E";
                    tEdit.Properties.ValueUnchecked = "H";
                    tEdit.Properties.EndInit();
                }


                //tEdit.Properties.ValueChecked = (byte)1;
                //tEdit.Properties.ValueGrayed = (byte)xx;
                //tEdit.Properties.ValueUnchecked = (byte)0;

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region CheckedListBoxControl
            if (tcolumn_type == "CheckedListBoxControl")
            {
            }
            #endregion

            #region ComboBoxEdit
            if (tcolumn_type == "ComboBoxEdit")
            {
                DevExpress.XtraEditors.ComboBoxEdit tEdit = new DevExpress.XtraEditors.ComboBoxEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                
                //tEdit.Properties.Items.Add();

                XtraEditorsComboBox_Fill(tEdit, row_Fields, "", 1); // Tumu = hayır

                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));
                if (tDLayout != null)
                    tEdit.StyleController = tDLayout;

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region DateEdit
            if ((tcolumn_type == "DateEdit") ||
                (tcolumn_type == "DateEdit_SpeedKriter_BAS") ||
                (tcolumn_type == "DateEdit_SpeedKriter_BIT"))
            {
                DateEdit tEdit = new DevExpress.XtraEditors.DateEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));
                if (tDLayout != null)
                    tEdit.StyleController = tDLayout;

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                if (t.IsNotNull(tdisplayformat))
                {
                    //column.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
                    // çalışmadı                                       
                    
                    //-1,  'not'
                    // 0,  'none'
                    // 1,  'Numeric'
                    // 2,  'DateTime'
                    // 3,  'Custom'
                    tcmp_format_type = t.Set(row_Fields["CMP_FORMAT_TYPE"].ToString(), row_Fields["LKP_CMP_FORMAT_TYPE"].ToString(), (Int16)0);
                    
                    tEdit.Properties.DisplayFormat.FormatString = tdisplayformat;
                    if (tcmp_format_type == 2) tEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    if (tcmp_format_type == 3) tEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

                    tEdit.Properties.EditFormat.FormatString = tdisplayformat;
                    if (tcmp_format_type == 2) tEdit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    if (tcmp_format_type == 3) tEdit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;

                    tEdit.Properties.Mask.EditMask = tdisplayformat;
                    tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
                }


                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);

                if (tview_type == 2)
                {
                    if (tcolumn_type == "DateEdit_SpeedKriter_BAS") tEdit.Name = tEdit.Name + "_BAS"; 
                    if (tcolumn_type == "DateEdit_SpeedKriter_BIT") tEdit.Name = tEdit.Name + "_BIT";

                    tEdit.EditValueChanged += new System.EventHandler(ev.tXtraEdit_ImageComboBoxEdit_EditValueChanged);
                    tEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(ev.tXtraEdit_EditValueChanging);
                }
            }
            #endregion

            #region HyperLinkEdit
            if (tcolumn_type == "HyperLinkEdit")
            {
                HyperLinkEdit tEdit = new DevExpress.XtraEditors.HyperLinkEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));
                if (tDLayout != null)
                    tEdit.StyleController = tDLayout;

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region ImageComboBoxEdit
            if ((tcolumn_type == "ImageComboBoxEdit") ||
                (tcolumn_type == "tImageComboBoxEdit2Button") ||
                (tcolumn_type == "tImageComboBoxEditSEC") ||
                (tcolumn_type == "tImageComboBoxEditSubView")
                )
            {
                ImageComboBoxEdit tEdit = new DevExpress.XtraEditors.ImageComboBoxEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));
                if (tDLayout != null)
                    tEdit.StyleController = tDLayout;

                XtraEditorsImageComboBox_Fill(tEdit, row_Fields, default_value, tview_type);
                
                // unutma
                // yukarıda bir yerde bu bilgi uçuyor bul onu
                tEdit.Properties.AccessibleName = TableIPCode;

                if (tcolumn_type == "tImageComboBoxEdit2Button")
                {
                    DevExpress.XtraEditors.Controls.EditorButton tBtn =
                    new DevExpress.XtraEditors.Controls.EditorButton();

                    tBtn.Caption = tFieldName;

                    tEdit.Properties.Buttons.Add(tBtn);
                    tEdit.ButtonClick += new
                       DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.ImageComboBoxEdit_ButtonClick);
                    // DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);
                    
                    #region tSearchEdit
                    //if (tcolumn_type == "tSearchEdit")
                    //{
                    tProp_Navigator = "Type:" + v.SearchEngine + ";" + tProp_Navigator;
                        tEdit.Properties.AccessibleDescription = tProp_Navigator;
                        tEdit.Properties.Buttons[1].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Search;
                    //}
                    #endregion tSearchEdit
                }

                if (tcolumn_type == "tImageComboBoxEditSEC")
                {
                    DevExpress.XtraEditors.Controls.EditorButton tBtn =
                       new DevExpress.XtraEditors.Controls.EditorButton();

                    tBtn.Caption = "SEÇ";
                    tBtn.Width = 30;
                    tBtn.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Search;

                    tProp_Navigator = "Type:" + v.SearchEngine + ";" + tProp_Navigator;
                    tEdit.Properties.AccessibleDescription = tProp_Navigator;
                    tEdit.Properties.ReadOnly = true;
                    tEdit.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
                    tEdit.Properties.Buttons[0].Visible = false;
                    tEdit.Properties.Buttons.Add(tBtn);
                    
                    //tEdit.ButtonClick += new
                    //  DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.ImageComboBoxEdit_ButtonClick);
                    tEdit.ButtonClick += new
                      DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);
                }

                if (tcolumn_type == "tImageComboBoxEditSubView")
                {
                    /// -----
                    /// xtraEdit 
                    tEdit.EditValueChanged += new System.EventHandler(ev.tXtraEdit_ImageComboBoxEdit_EditValueChanged);
                }

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);

                if (tview_type == 2)
                {
                    tEdit.EditValueChanged += new System.EventHandler(ev.tXtraEdit_ImageComboBoxEdit_EditValueChanged);
                    //tEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(ev.tXtraEdit_EditValueChanging);
                }
            
            }
            #endregion

            #region ImageEdit
            if (tcolumn_type == "ImageEdit")
            {
                ImageEdit tEdit = new DevExpress.XtraEditors.ImageEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region ImageListBoxControl
            if (tcolumn_type == "ImageListBoxControl")
            {
                ImageListBoxControl tEdit = new DevExpress.XtraEditors.ImageListBoxControl();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.AccessibleName = TableIPCode;
                //Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region LabelControl
            if (tcolumn_type == "LabelControl")
            {
                LabelControl tEdit = new DevExpress.XtraEditors.LabelControl();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("Text", dsData.Tables[0], tFieldName));
                //tEdit.l
                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region ListBoxControl
            if (tcolumn_type == "ListBoxControl")
            {

            }
            #endregion

            #region LookUpEdit
            if (tcolumn_type == "LookUpEdit")
            {
                LookUpEdit tEdit = new DevExpress.XtraEditors.LookUpEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region MemoEdit
            if (tcolumn_type == "MemoEdit")
            {
                MemoEdit tEdit = new DevExpress.XtraEditors.MemoEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));
                if (tDLayout != null)
                    tEdit.StyleController = tDLayout;

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region MemoExEdit
            if (tcolumn_type == "MemoExEdit")
            {
                MemoExEdit tEdit = new DevExpress.XtraEditors.MemoExEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));
                if (tDLayout != null)
                    tEdit.StyleController = tDLayout;

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region MRUEdit
            if (tcolumn_type == "MRUEdit")
            {
                MRUEdit tEdit = new DevExpress.XtraEditors.MRUEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region PictureEdit
            if (tcolumn_type == "PictureEdit")
            {
                PictureEdit tEdit = new DevExpress.XtraEditors.PictureEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));

                //System.Windows.Forms.MenuItem mitem = new System.Windows.Forms.MenuItem();
                //mitem.Text = "aaaa";
                //tEdit.Properties.ContextMenu.MenuItems.Add(mitem);

                int min = t.myInt32(row_Fields["VALIDATION_VALUE1"].ToString());
                int max = t.myInt32(row_Fields["VALIDATION_VALUE2"].ToString());

                tEdit.Properties.AccessibleDefaultActionDescription = min.ToString() + "." + max.ToString();
                
                //if (Column.ToString() == s)
                //    ((LayoutControlItem)Column).Control = tEdit;
                //else ((Control)Column).Controls.Add(tEdit);

                if (Column.ToString() == s)
                {
                    ((LayoutControlItem)Column).TextVisible = false; // caption olamsın
                    ((LayoutControlItem)Column).Control = Create_UserPictureControl(tEdit, tFieldName, FormName);
                }
                else ((Control)Column).Controls.Add(Create_UserPictureControl(tEdit, tFieldName, FormName));
                

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region PopupContainerControl
            if (tcolumn_type == "PopupContainerControl")
            {
                /*
                  RichTextBox rtb = new RichTextBox();
                  rtb.Dock = DockStyle.Fill;
                  PopupContainerControl popupControl = new PopupContainerControl();
                  popupControl.Controls.Add(rtb);

                  PopupContainerEdit editor = new PopupContainerEdit();
                  editor.Properties.PopupControl = popupControl;
                  Controls.Add(editor);
               */

                RichTextBox rtb = new RichTextBox();
                rtb.Dock = DockStyle.Fill;
                PopupContainerControl popupControl = new PopupContainerControl();
                popupControl.Controls.Add(rtb);

                PopupContainerEdit tEdit = new PopupContainerEdit();
                tEdit.Properties.PopupControl = popupControl;
                //Controls.Add(tEdit);

                //if (dsData != null)
                //    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

            }
            #endregion

            #region PopupContainerEdit
            if (tcolumn_type == "PopupContainerEdit")
            {
            }
            #endregion

            #region RadioGroup
            if (tcolumn_type == "RadioGroup")
            {
                RadioGroup tEdit = new DevExpress.XtraEditors.RadioGroup();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));

                RepositoryItemRadioGroup_Fill(tEdit, row_Fields, "", tview_type);

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region RangeTrackBarControl
            if (tcolumn_type == "RangeTrackBarControl")
            {
            }
            #endregion

            #region SpinEdit
            if (tcolumn_type == "SpinEdit")
            {
                SpinEdit tEdit = new DevExpress.XtraEditors.SpinEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));
                if (tDLayout != null)
                    tEdit.StyleController = tDLayout;

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region TextEdit
            if (tcolumn_type == "TextEdit")
            {
                TextEdit tEdit = new DevExpress.XtraEditors.TextEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                
                //tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));
                if (tDLayout != null)
                    tEdit.StyleController = tDLayout;

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                #region displayformat
                if (t.IsNotNull(tdisplayformat))
                {
                    XtraEditorsTextEdit_DisplayFormat(tEdit, tdisplayformat, tcmp_format_type);
                }
                #endregion displayformat

                #region tExpressionType
                if (tExpressionType > 0)
                {
                    tEdit.Properties.Tag = "EXPRESSION";
                    tEdit.EditValueChanged += new System.EventHandler(ev.tXtraEdit_EditValueChanged);
                    tEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(ev.tXtraEdit_EditValueChanging);
                    tEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(ev.tXtraEdit_KeyDown);
                    tEdit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(ev.tXtraEdit_KeyPress);
                    tEdit.KeyUp += new System.Windows.Forms.KeyEventHandler(ev.tXtraEdit_KeyUp);
                }
                #endregion

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);

            }
            #endregion

            #region TimeEdit
            if (tcolumn_type == "TimeEdit")
            {
                TimeEdit tEdit = new DevExpress.XtraEditors.TimeEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));
                if (tDLayout != null)
                    tEdit.StyleController = tDLayout;

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                if (t.IsNotNull(tdisplayformat))
                    tEdit.Properties.DisplayFormat.FormatString = tdisplayformat;

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion

            #region TimeSpanEdit
            if (tcolumn_type == "TimeSpanEdit")
            {
                TimeSpanEdit tEdit = new TimeSpanEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AllowEditDays = false;

                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));
                if (tDLayout != null)
                    tEdit.StyleController = tDLayout;

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                // ???? 
                //Column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                
            }
            #endregion
            
            #region TrackBarControl
            if (tcolumn_type == "TrackBarControl")
            {
                TrackBarControl tEdit = new DevExpress.XtraEditors.TrackBarControl();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;
                tEdit.Tag = field_no;
                if (dsData != null)
                    tEdit.DataBindings.Add(new Binding("EditValue", dsData.Tables[0], tFieldName));

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tXtraEditors_Properties(row_Fields, tEdit, tcolumn_type, tview_type);
            }
            #endregion
            
            #region ToggleSwitch
            if (tcolumn_type == "ToggleSwitch")
            {
                ToggleSwitch tEdit = new DevExpress.XtraEditors.ToggleSwitch();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Properties.AccessibleName = TableIPCode;

                if (Column.ToString() == s)
                    ((LayoutControlItem)Column).Control = tEdit;
                else ((Control)Column).Controls.Add(tEdit);

                tEdit.Dock = DockStyle.Fill;

                bool tenabled = t.Set(row_Fields["KRT_LIKE"].ToString(), "", false);
                if (tenabled)
                {
                    tEdit.EditValue = true;
                }

                if (tview_type == 2)
                {
                    tEdit.EditValueChanged += new System.EventHandler(ev.tXtraEdit_ToggleSwitch_EditValueChanged);
                    //tEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(ev.tXtraEdit_EditValueChanging);
                }

            }
            #endregion ToggleSwitch

        }

        private void tXtraEditors_Properties(DataRow row_Fields, Object tEdit, string tcolumn_type, byte tview_type)
        {
            if (row_Fields == null) return;

            tToolBox t = new tToolBox();

            //int tvalue = 0;
            //int vld_operator = 0;
            Boolean tenabled = true;

            string tTableName = row_Fields["LKP_TABLE_NAME"].ToString();
            string tFieldName = row_Fields["LKP_FIELD_NAME"].ToString();
            int field_no = t.myInt32(row_Fields["FIELD_NO"].ToString());
            int vld_operator = t.Set(row_Fields["VALIDATION_OPERATOR"].ToString(), row_Fields["LKP_VALIDATION_OPERATOR"].ToString(), (int)150);

            #region dxValidationProvider

            if (vld_operator > 11)
            {
                if (tcolumn_type == "ButtonEdit")
                    ((DevExpress.XtraEditors.ButtonEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "CalcEdit")
                    ((DevExpress.XtraEditors.CalcEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "CheckButton")
                    ((DevExpress.XtraEditors.CheckButton)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "CheckedComboBoxEdit")
                    ((DevExpress.XtraEditors.CheckedComboBoxEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "CheckEdit")
                    ((DevExpress.XtraEditors.CheckEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "CheckedListBoxControl")
                    ((DevExpress.XtraEditors.CheckedListBoxControl)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "ComboBoxEdit")
                    ((DevExpress.XtraEditors.ComboBoxEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "DateEdit")
                    ((DevExpress.XtraEditors.DateEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "HyperLinkEdit")
                    ((DevExpress.XtraEditors.HyperLinkEdit)tEdit).BackColor = v.Validate_New;
                if ((tcolumn_type == "ImageComboBoxEdit") ||
                    (tcolumn_type == "tImageComboBoxEdit2Button") ||
                    (tcolumn_type == "tImageComboBoxEditSEC") ||
                    (tcolumn_type == "tImageComboBoxEditSubView")
                    )
                    ((DevExpress.XtraEditors.ImageComboBoxEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "ImageEdit")
                    ((DevExpress.XtraEditors.ImageEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "ImageListBoxControl")
                    ((DevExpress.XtraEditors.ImageListBoxControl)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "LabelControl")
                    ((DevExpress.XtraEditors.LabelControl)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "ListBoxControl")
                    ((DevExpress.XtraEditors.ListBoxControl)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "LookUpEdit")
                    ((DevExpress.XtraEditors.LookUpEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "MemoEdit")
                    ((DevExpress.XtraEditors.MemoEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "MemoExEdit")
                    ((DevExpress.XtraEditors.MemoExEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "MRUEdit")
                    ((DevExpress.XtraEditors.MemoExEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "PictureEdit")
                    ((DevExpress.XtraEditors.PictureEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "PopupContainerControl")
                    ((DevExpress.XtraEditors.PopupContainerControl)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "PopupContainerEdit")
                    ((DevExpress.XtraEditors.PopupContainerEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "RadioGroup")
                    ((DevExpress.XtraEditors.RadioGroup)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "RangeTrackBarControl")
                    ((DevExpress.XtraEditors.RangeTrackBarControl)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "SpinEdit")
                    ((DevExpress.XtraEditors.SpinEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "TextEdit")
                    ((DevExpress.XtraEditors.TextEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "TimeEdit")
                    ((DevExpress.XtraEditors.TimeEdit)tEdit).BackColor = v.Validate_New;
                if (tcolumn_type == "TrackBarControl")
                    ((DevExpress.XtraEditors.TrackBarControl)tEdit).BackColor = v.Validate_New;



                #region dxValidationProvider denemesi
                /*
                if (sp.con_Validation != null)
                {
                    string Value1 = string.Empty;
                    string Value2 = string.Empty;
                    string ErrText = string.Empty;
                    Int16 ErrType = 0;
                    Int16 Operator = 0;

                    Operator = lkp_vld_operator;
                    if (vld_operator > 11) 
                        Operator = vld_operator;


                    if (vld_operator > 0)
                    {
                        Value1 = tp.IsNull(ds.Tables[0].Rows[row]["VALIDATION_VALUE1"].ToString(), "");
                        Value2 = tp.IsNull(ds.Tables[0].Rows[row]["VALIDATION_VALUE2"].ToString(), "");
                        ErrText = tp.IsNull(ds.Tables[0].Rows[row]["VALIDATION_ERRORTEXT"].ToString(), "Hata");
                        ErrType = Convert.ToInt16(tp.IsNull(ds.Tables[0].Rows[row]["VALIDATION_ERRORTYPE"].ToString(), "0"));
                    }
                    else
                    {
                        Value1 = tp.IsNull(ds.Tables[0].Rows[row]["LKP_VALIDATION_VALUE1"].ToString(), "");
                        Value2 = tp.IsNull(ds.Tables[0].Rows[row]["LKP_VALIDATION_VALUE2"].ToString(), "");
                        ErrText = tp.IsNull(ds.Tables[0].Rows[row]["LKP_VALIDATION_ERRORTEXT"].ToString(), "Hata");
                        ErrType = Convert.ToInt16(tp.IsNull(ds.Tables[0].Rows[row]["LKP_VALIDATION_ERRORTYPE"].ToString(), "0"));
                    }

                    ConditionValidationRule ValidationRule = new ConditionValidationRule();

                    if (Operator == 12) ValidationRule.ConditionOperator = ConditionOperator.AnyOf;
                    if (Operator == 13) ValidationRule.ConditionOperator = ConditionOperator.BeginsWith;
                    if (Operator == 14) ValidationRule.ConditionOperator = ConditionOperator.Between;
                    if (Operator == 15) ValidationRule.ConditionOperator = ConditionOperator.Contains;
                    if (Operator == 16) ValidationRule.ConditionOperator = ConditionOperator.EndsWith;
                    if (Operator == 17) ValidationRule.ConditionOperator = ConditionOperator.Equals;
                    if (Operator == 18) ValidationRule.ConditionOperator = ConditionOperator.Greater;
                    if (Operator == 19) ValidationRule.ConditionOperator = ConditionOperator.GreaterOrEqual;
                    if (Operator == 20) ValidationRule.ConditionOperator = ConditionOperator.IsBlank;    
                    if (Operator == 21) ValidationRule.ConditionOperator = ConditionOperator.IsNotBlank;    
                    if (Operator == 22) ValidationRule.ConditionOperator = ConditionOperator.Less;    
                    if (Operator == 23) ValidationRule.ConditionOperator = ConditionOperator.LessOrEqual;
                    if (Operator == 24) ValidationRule.ConditionOperator = ConditionOperator.Like;    
                    if (Operator == 25) ValidationRule.ConditionOperator = ConditionOperator.None;
                    if (Operator == 26) ValidationRule.ConditionOperator = ConditionOperator.NotAnyOf;
                    if (Operator == 27) ValidationRule.ConditionOperator = ConditionOperator.NotBetween;
                    if (Operator == 28) ValidationRule.ConditionOperator = ConditionOperator.NotContains;
                    if (Operator == 29) ValidationRule.ConditionOperator = ConditionOperator.NotEquals;
                    if (Operator == 30) ValidationRule.ConditionOperator = ConditionOperator.NotLike;

                    ValidationRule.Value1 = Value1;
                    ValidationRule.Value2 = Value2;
                    ValidationRule.ErrorText = ErrText;
                    ValidationRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical;   

                    if (tcolumn_type == "ButtonEdit")
                    {
                        sp.con_Validation.SetValidationRule(((DevExpress.XtraEditors.ButtonEdit)tEdit), ValidationRule);
                    }

                    if (tcolumn_type == "CalcEdit")
                    {
                        sp.con_Validation.SetValidationRule(((DevExpress.XtraEditors.CalcEdit)tEdit), ValidationRule);
                    }

                    if (tcolumn_type == "RadioGroup")
                    {
                        sp.con_Validation.SetValidationRule(((DevExpress.XtraEditors.RadioGroup)tEdit), ValidationRule);
                    }
                }
                * * 
                */
                #endregion dxValidationProvider denemesi

                #region  xxxxx.Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                /*
                if (tcolumn_type == "ButtonEdit")
                    ((DevExpress.XtraEditors.ButtonEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "CalcEdit")
                    ((DevExpress.XtraEditors.CalcEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "CheckButton")
                    ((DevExpress.XtraEditors.CheckButton)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "CheckedComboBoxEdit")
                    ((DevExpress.XtraEditors.CheckedComboBoxEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "CheckEdit")
                    ((DevExpress.XtraEditors.CheckEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "CheckedListBoxControl")
                    ((DevExpress.XtraEditors.CheckedListBoxControl)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "ComboBoxEdit")
                    ((DevExpress.XtraEditors.ComboBoxEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "DateEdit")
                    ((DevExpress.XtraEditors.DateEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "HyperLinkEdit")
                    ((DevExpress.XtraEditors.HyperLinkEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "ImageComboBoxEdit")
                    ((DevExpress.XtraEditors.ImageComboBoxEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "ImageEdit")
                    ((DevExpress.XtraEditors.ImageEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "ImageListBoxControl")
                    ((DevExpress.XtraEditors.ImageListBoxControl)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "LabelControl")
                    ((DevExpress.XtraEditors.LabelControl)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "ListBoxControl")
                    ((DevExpress.XtraEditors.ListBoxControl)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "LookUpEdit")
                    ((DevExpress.XtraEditors.LookUpEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "MemoEdit")
                    ((DevExpress.XtraEditors.MemoEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "MemoExEdit")
                    ((DevExpress.XtraEditors.MemoExEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "MRUEdit")
                    ((DevExpress.XtraEditors.MemoExEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "PictureEdit")
                    ((DevExpress.XtraEditors.PictureEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "PopupContainerControl")
                    ((DevExpress.XtraEditors.PopupContainerControl)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "PopupContainerEdit")
                    ((DevExpress.XtraEditors.PopupContainerEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "RadioGroup")
                    ((DevExpress.XtraEditors.RadioGroup)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "RangeTrackBarControl")
                    ((DevExpress.XtraEditors.RangeTrackBarControl)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "SpinEdit")
                    ((DevExpress.XtraEditors.SpinEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "TextEdit")
                    ((DevExpress.XtraEditors.TextEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "TimeEdit")
                    ((DevExpress.XtraEditors.TimeEdit)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                if (tcolumn_type == "TrackBarControl")
                    ((DevExpress.XtraEditors.TrackBarControl)tEdit).Validating += new System.ComponentModel.CancelEventHandler(DevExpCC.myEdit_Validating);
                */
                #endregion

            } // if ((lkp_vld_operator > 11)

            #endregion Validation

            #region Enabled = false

            tenabled = t.Set(row_Fields["CMP_ENABLED"].ToString(), row_Fields["LKP_FENABLED"].ToString(), true);
                        
            if ((tenabled == false) && (tview_type != 2)) // SpeedKriter değilse
            {
                if (tcolumn_type == "ButtonEdit")
                    ((DevExpress.XtraEditors.ButtonEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "CalcEdit")
                    ((DevExpress.XtraEditors.CalcEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "CheckButton")
                    ((DevExpress.XtraEditors.CheckButton)tEdit).Enabled = false;
                if (tcolumn_type == "CheckedComboBoxEdit")
                    ((DevExpress.XtraEditors.CheckedComboBoxEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "CheckEdit")
                    ((DevExpress.XtraEditors.CheckEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "CheckedListBoxControl")
                    ((DevExpress.XtraEditors.CheckedListBoxControl)tEdit).Enabled = false;
                if (tcolumn_type == "ComboBoxEdit")
                    ((DevExpress.XtraEditors.ComboBoxEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "DateEdit")
                    ((DevExpress.XtraEditors.DateEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "HyperLinkEdit")
                    ((DevExpress.XtraEditors.HyperLinkEdit)tEdit).Properties.ReadOnly = true;
                if ((tcolumn_type == "ImageComboBoxEdit") ||
                    (tcolumn_type == "tImageComboBoxEdit2Button") ||
                    (tcolumn_type == "tImageComboBoxEditSEC") ||
                    (tcolumn_type == "tImageComboBoxEditSubView")
                    )
                    ((DevExpress.XtraEditors.ImageComboBoxEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "ImageEdit")
                    ((DevExpress.XtraEditors.ImageEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "ImageListBoxControl")
                    ((DevExpress.XtraEditors.ImageListBoxControl)tEdit).Enabled = false;
                if (tcolumn_type == "LabelControl")
                    ((DevExpress.XtraEditors.LabelControl)tEdit).Enabled = false;
                if (tcolumn_type == "ListBoxControl")
                    ((DevExpress.XtraEditors.ListBoxControl)tEdit).Enabled = false;
                if (tcolumn_type == "LookUpEdit")
                    ((DevExpress.XtraEditors.LookUpEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "MemoEdit")
                    ((DevExpress.XtraEditors.MemoEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "MemoExEdit")
                    ((DevExpress.XtraEditors.MemoExEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "MRUEdit")
                    ((DevExpress.XtraEditors.MemoExEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "PictureEdit")
                    ((DevExpress.XtraEditors.PictureEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "PopupContainerControl")
                    ((DevExpress.XtraEditors.PopupContainerControl)tEdit).Enabled = false;
                if (tcolumn_type == "PopupContainerEdit")
                    ((DevExpress.XtraEditors.PopupContainerEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "RadioGroup")
                    ((DevExpress.XtraEditors.RadioGroup)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "RangeTrackBarControl")
                    ((DevExpress.XtraEditors.RangeTrackBarControl)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "SpinEdit")
                    ((DevExpress.XtraEditors.SpinEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "TextEdit")
                    ((DevExpress.XtraEditors.TextEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "TimeEdit")
                    ((DevExpress.XtraEditors.TimeEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "TimeSpanEdit")
                    ((DevExpress.XtraEditors.TimeSpanEdit)tEdit).Properties.ReadOnly = true;
                if (tcolumn_type == "TrackBarControl")
                    ((DevExpress.XtraEditors.TrackBarControl)tEdit).Properties.ReadOnly = true;
            }
            #endregion
            
            #region Width
            /*
            tvalue = t.Set(row_Fields["CMP_WIDTH"].ToString(), row_Fields["LKP_CMP_WIDTH"].ToString(), (int)150);

            if (tvalue != 0)
            {
                if (tcolumn_type == "ButtonEdit")
                    ((DevExpress.XtraEditors.ButtonEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "CalcEdit")
                    ((DevExpress.XtraEditors.CalcEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "CheckButton")
                    ((DevExpress.XtraEditors.CheckButton)tEdit).Width = tvalue;
                if (tcolumn_type == "CheckedComboBoxEdit")
                    ((DevExpress.XtraEditors.CheckedComboBoxEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "CheckEdit")
                    ((DevExpress.XtraEditors.CheckEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "CheckedListBoxControl")
                    ((DevExpress.XtraEditors.CheckedListBoxControl)tEdit).Width = tvalue;
                if (tcolumn_type == "ComboBoxEdit")
                    ((DevExpress.XtraEditors.ComboBoxEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "DateEdit")
                    ((DevExpress.XtraEditors.DateEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "HyperLinkEdit")
                    ((DevExpress.XtraEditors.HyperLinkEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "ImageComboBoxEdit")
                    ((DevExpress.XtraEditors.ImageComboBoxEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "ImageEdit")
                    ((DevExpress.XtraEditors.ImageEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "ImageListBoxControl")
                    ((DevExpress.XtraEditors.ImageListBoxControl)tEdit).Width = tvalue;
                if (tcolumn_type == "LabelControl")
                    ((DevExpress.XtraEditors.LabelControl)tEdit).Width = tvalue;
                if (tcolumn_type == "ListBoxControl")
                    ((DevExpress.XtraEditors.ListBoxControl)tEdit).Width = tvalue;
                if (tcolumn_type == "LookUpEdit")
                    ((DevExpress.XtraEditors.LookUpEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "MemoEdit")
                    ((DevExpress.XtraEditors.MemoEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "MemoExEdit")
                    ((DevExpress.XtraEditors.MemoExEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "MRUEdit")
                    ((DevExpress.XtraEditors.MemoExEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "PictureEdit")
                    ((DevExpress.XtraEditors.PictureEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "PopupContainerControl")
                    ((DevExpress.XtraEditors.PopupContainerControl)tEdit).Width = tvalue;
                if (tcolumn_type == "PopupContainerEdit")
                    ((DevExpress.XtraEditors.PopupContainerEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "RadioGroup")
                    ((DevExpress.XtraEditors.RadioGroup)tEdit).Width = tvalue;
                if (tcolumn_type == "RangeTrackBarControl")
                    ((DevExpress.XtraEditors.RangeTrackBarControl)tEdit).Width = tvalue;
                if (tcolumn_type == "SpinEdit")
                    ((DevExpress.XtraEditors.SpinEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "TextEdit")
                    ((DevExpress.XtraEditors.TextEdit)tEdit).Width = tvalue;
                if (tcolumn_type == "TimeEdit")
                    ((DevExpress.XtraEditors.TimeEdit)tEdit).Width = tvalue;

                if (tcolumn_type == "TrackBarControl")
                    ((DevExpress.XtraEditors.TrackBarControl)tEdit).Width = tvalue;

            }*/
            #endregion Width
            
            #region Height
            /*
            tvalue = t.Set(row_Fields["CMP_HEIGHT"].ToString(), "", (int)150);
            
            if (tvalue != 0)
            {
                if (tcolumn_type == "ButtonEdit")
                    ((DevExpress.XtraEditors.ButtonEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "CalcEdit")
                    ((DevExpress.XtraEditors.CalcEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "CheckButton")
                    ((DevExpress.XtraEditors.CheckButton)tEdit).Height = tvalue;
                if (tcolumn_type == "CheckedComboBoxEdit")
                    ((DevExpress.XtraEditors.CheckedComboBoxEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "CheckEdit")
                    ((DevExpress.XtraEditors.CheckEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "CheckedListBoxControl")
                    ((DevExpress.XtraEditors.CheckedListBoxControl)tEdit).Height = tvalue;
                if (tcolumn_type == "ComboBoxEdit")
                    ((DevExpress.XtraEditors.ComboBoxEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "DateEdit")
                    ((DevExpress.XtraEditors.DateEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "HyperLinkEdit")
                    ((DevExpress.XtraEditors.HyperLinkEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "ImageComboBoxEdit")
                    ((DevExpress.XtraEditors.ImageComboBoxEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "ImageEdit")
                    ((DevExpress.XtraEditors.ImageEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "ImageListBoxControl")
                    ((DevExpress.XtraEditors.ImageListBoxControl)tEdit).Height = tvalue;
                if (tcolumn_type == "LabelControl")
                    ((DevExpress.XtraEditors.LabelControl)tEdit).Height = tvalue;
                if (tcolumn_type == "ListBoxControl")
                    ((DevExpress.XtraEditors.ListBoxControl)tEdit).Height = tvalue;
                if (tcolumn_type == "LookUpEdit")
                    ((DevExpress.XtraEditors.LookUpEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "MemoEdit")
                    ((DevExpress.XtraEditors.MemoEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "MemoExEdit")
                    ((DevExpress.XtraEditors.MemoExEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "MRUEdit")
                    ((DevExpress.XtraEditors.MemoExEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "PictureEdit")
                    ((DevExpress.XtraEditors.PictureEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "PopupContainerControl")
                    ((DevExpress.XtraEditors.PopupContainerControl)tEdit).Height = tvalue;
                if (tcolumn_type == "PopupContainerEdit")
                    ((DevExpress.XtraEditors.PopupContainerEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "RadioGroup")
                    ((DevExpress.XtraEditors.RadioGroup)tEdit).Height = tvalue;
                if (tcolumn_type == "RangeTrackBarControl")
                    ((DevExpress.XtraEditors.RangeTrackBarControl)tEdit).Height = tvalue;
                if (tcolumn_type == "SpinEdit")
                    ((DevExpress.XtraEditors.SpinEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "TextEdit")
                    ((DevExpress.XtraEditors.TextEdit)tEdit).Height = tvalue;
                if (tcolumn_type == "TimeEdit")
                    ((DevExpress.XtraEditors.TimeEdit)tEdit).Height = tvalue;

                if (tcolumn_type == "TrackBarControl")
                    ((DevExpress.XtraEditors.TrackBarControl)tEdit).Height = tvalue;

            }   */
            #endregion Height

            #region Font_Size
            float font_size = 0;

            font_size = t.Set(row_Fields["CMP_FONT_SIZE"].ToString(), "", (float)0);
            
            if (font_size > 0)
            {
                font_size = font_size + .25F;

                if (font_size > .25)
                {
                    if ((tcolumn_type == "ButtonEdit") || tcolumn_type == "tSearchEdit")
                        ((DevExpress.XtraEditors.ButtonEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "CalcEdit")
                        ((DevExpress.XtraEditors.CalcEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "CheckButton")
                        ((DevExpress.XtraEditors.CheckButton)tEdit).Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "CheckedComboBoxEdit")
                        ((DevExpress.XtraEditors.CheckedComboBoxEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "CheckEdit")
                        ((DevExpress.XtraEditors.CheckEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "CheckedListBoxControl")
                        ((DevExpress.XtraEditors.CheckedListBoxControl)tEdit).Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "ComboBoxEdit")
                        ((DevExpress.XtraEditors.ComboBoxEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "DateEdit")
                        ((DevExpress.XtraEditors.DateEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "HyperLinkEdit")
                        ((DevExpress.XtraEditors.HyperLinkEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if ((tcolumn_type == "ImageComboBoxEdit") ||
                        (tcolumn_type == "tImageComboBoxEdit2Button") ||
                        (tcolumn_type == "tImageComboBoxEditSEC") ||
                        (tcolumn_type == "tImageComboBoxEditSubView")
                        )
                        ((DevExpress.XtraEditors.ImageComboBoxEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "ImageEdit")
                        ((DevExpress.XtraEditors.ImageEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "ImageListBoxControl")
                        ((DevExpress.XtraEditors.ImageListBoxControl)tEdit).Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "LabelControl")
                        ((DevExpress.XtraEditors.LabelControl)tEdit).Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "ListBoxControl")
                        ((DevExpress.XtraEditors.ListBoxControl)tEdit).Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "LookUpEdit")
                        ((DevExpress.XtraEditors.LookUpEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "MemoEdit")
                        ((DevExpress.XtraEditors.MemoEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "MemoExEdit")
                        ((DevExpress.XtraEditors.MemoExEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "MRUEdit")
                        ((DevExpress.XtraEditors.MemoExEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "PictureEdit")
                        ((DevExpress.XtraEditors.PictureEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "PopupContainerControl")
                        ((DevExpress.XtraEditors.PopupContainerControl)tEdit).Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "PopupContainerEdit")
                        ((DevExpress.XtraEditors.PopupContainerEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "RadioGroup")
                        ((DevExpress.XtraEditors.RadioGroup)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "RangeTrackBarControl")
                        ((DevExpress.XtraEditors.RangeTrackBarControl)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "SpinEdit")
                        ((DevExpress.XtraEditors.SpinEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "TextEdit")
                        ((DevExpress.XtraEditors.TextEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "TimeEdit")
                        ((DevExpress.XtraEditors.TimeEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "TimeSpanEdit")
                        ((DevExpress.XtraEditors.TimeSpanEdit)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                    if (tcolumn_type == "TrackBarControl")
                        ((DevExpress.XtraEditors.TrackBarControl)tEdit).Properties.Appearance.Font = new Font("Tahoma", font_size);
                }
            }
            //this.textEdit1.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F);
            //this.textEdit1.Properties.Appearance.Options.UseFont = true;
            #endregion Font Size

            
        }
        
        #endregion
        
        #region Tree_ColumnEdit

        public void Tree_ColumnEdit(DataRow Row, DevExpress.XtraTreeList.Columns.TreeListColumn Column, string tcolumn_type)
        {
            tToolBox t = new tToolBox();
            tEvents ev = new tEvents();

            string tTableName = Row["LKP_TABLE_NAME"].ToString();
            string tFieldName = Row["LKP_FIELD_NAME"].ToString();
            string tExpression = Row["LKP_PROP_EXPRESSION"].ToString();
            // formülleri çalıştırmak için etkisi varmı
            // 0 = yok
            // 1 = true
            Int16 tExpressionType = t.myInt16(Row["LKP_EXPRESSION_TYPE"].ToString());
            float font_size = t.Set(Row["CMP_FONT_SIZE"].ToString(), "", (float)0);
            string tdisplayformat = t.Set(Row["CMP_DISPLAY_FORMAT"].ToString(), Row["LKP_CMP_DISPLAY_FORMAT"].ToString(), "");
            string tProp_Navigator = t.Set(Row["PROP_NAVIGATOR"].ToString(), "", "");
            

            #region tPropertiesEdit
            if (tcolumn_type == "tPropertiesEdit")
            {
                RepositoryItemButtonEdit tEdit = new RepositoryItemButtonEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                tEdit.KeyDown += new KeyEventHandler(ev.buttonEdit_KeyDown);
                tEdit.KeyPress += new KeyPressEventHandler(ev.buttonEdit_KeyPress);
                tEdit.KeyUp += new KeyEventHandler(ev.buttonEdit_KeyUp);
                tEdit.Leave += new EventHandler(ev.buttonEdit_Leave);
                tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);

                Int16 width = t.Set(Row["LKP_CMP_WIDTH"].ToString(), "", (Int16)200);

                string s = string.Empty;
                t.MyProperties_Set(ref s, "Type", v.Properties);
                t.MyProperties_Set(ref s, "TableName", tTableName);
                t.MyProperties_Set(ref s, "FieldName", tFieldName);
                t.MyProperties_Set(ref s, "Width", width.ToString());

                tEdit.AccessibleDescription = s;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);

                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region tPropertiesPlusEdit
            if (tcolumn_type == "tPropertiesPlusEdit")
            {
                RepositoryItemButtonEdit tEdit = new RepositoryItemButtonEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                tEdit.KeyDown += new KeyEventHandler(ev.buttonEdit_KeyDown);
                tEdit.KeyPress += new KeyPressEventHandler(ev.buttonEdit_KeyPress);
                tEdit.KeyUp += new KeyEventHandler(ev.buttonEdit_KeyUp);
                tEdit.Leave += new EventHandler(ev.buttonEdit_Leave);
                tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);

                Int16 width = t.Set(Row["LKP_CMP_WIDTH"].ToString(), "", (Int16)200);

                string s = string.Empty;
                t.MyProperties_Set(ref s, "Type", v.PropertiesPlus);
                t.MyProperties_Set(ref s, "TableName", tTableName);
                t.MyProperties_Set(ref s, "FieldName", tFieldName);
                t.MyProperties_Set(ref s, "Width", width.ToString());

                tEdit.AccessibleDescription = s;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);

                Column.ColumnEdit = tEdit;
            }
            #endregion
                        
            #region ButtonEdit
            if ((tcolumn_type == "ButtonEdit") ||
                    (tcolumn_type == "tSearchEdit"))
            {
                RepositoryItemButtonEdit tEdit = new RepositoryItemButtonEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                tEdit.KeyDown += new KeyEventHandler(ev.buttonEdit_KeyDown);
                tEdit.KeyPress += new KeyPressEventHandler(ev.buttonEdit_KeyPress);
                tEdit.KeyUp += new KeyEventHandler(ev.buttonEdit_KeyUp);
                tEdit.Leave += new EventHandler(ev.buttonEdit_Leave);
                tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);
                
                if (tFieldName == "LKP_KOMUT")
                {
                    tEdit.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.OK;
                    tEdit.Buttons[0].Width = 40; // Column.Width / 2;
                    tEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                }

                // Arama Motoru 
                /*
                // TLKP_TYPE, FLKP_TYPE 
                if (Row["LKP_FLKP_TYPE"] != null)
                {
                    tEdit.Tag = ev.ButtonEdit_AND(Row);
                    tEdit.AccessibleDescription = v.SearchEngine;
                    if (Row["LKP_PARTNER_FIELD_NAME"] != null)
                        tEdit.AccessibleDefaultActionDescription = Row["LKP_PARTNER_FIELD_NAME"].ToString();
                }
                 */

                #region tSearchEdit
                if (tcolumn_type == "tSearchEdit")
                {
                    tProp_Navigator = "Type:" + v.SearchEngine + ";" + tProp_Navigator;
                    tEdit.AccessibleDescription = tProp_Navigator;
                    tEdit.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Search;
                }
                #endregion tSearchEdit


                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region CalcEdit
            if (tcolumn_type == "CalcEdit")
            {
                RepositoryItemCalcEdit tEdit = new RepositoryItemCalcEdit();
                tEdit.Name = "Column_" + tFieldName;

                if (tExpressionType > 0)
                    Column.Tag = "EXPRESSION";
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region CheckButton
            if (tcolumn_type == "CheckButton")
            {
            }
            #endregion

            #region CheckedComboBoxEdit
            if (tcolumn_type == "CheckedComboBoxEdit")
            {
                RepositoryItemCheckedComboBoxEdit tEdit = new RepositoryItemCheckedComboBoxEdit();
                tEdit.Name = "Column_" + tFieldName;

                RepositoryItemCheckedComboBoxEdit_Fill(tEdit, Row);
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region CheckEdit
            if (tcolumn_type == "CheckEdit")
            {
                RepositoryItemCheckEdit tEdit = new RepositoryItemCheckEdit();
                tEdit.Name = "Column_" + tFieldName;

                byte ftype = t.Set(Row["LKP_FIELD_TYPE"].ToString(), "", (byte)0);

                if (ftype == 52) // Int16
                {
                    tEdit.BeginInit();
                    tEdit.ValueChecked = ((short)(1));
                    tEdit.ValueUnchecked = ((short)(0));
                    tEdit.EndInit();
                }

                if (ftype == 56) // int
                {
                    tEdit.BeginInit();
                    tEdit.ValueChecked = 1;
                    tEdit.ValueUnchecked = 0;
                    tEdit.EndInit();
                }
                if (ftype == 167) // varchar
                {
                    tEdit.BeginInit();
                    tEdit.ValueUnchecked = "";
                    tEdit.ValueChecked = "E";
                    tEdit.ValueUnchecked = "H";
                    tEdit.EndInit();
                }

                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region CheckedListBoxControl
            if (tcolumn_type == "CheckedListBoxControl")
            {
            }
            #endregion

            #region ComboBoxEdit
            if (tcolumn_type == "ComboBoxEdit")
            {
                RepositoryItemComboBox tEdit = new RepositoryItemComboBox();
                tEdit.Name = "Column_" + tFieldName;

                RepositoryItemComboBox_Fill(tEdit, Row, "", 1); // Tumu = hayır
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region DateEdit
            if (tcolumn_type == "DateEdit")
            {
                RepositoryItemDateEdit tEdit = new RepositoryItemDateEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region HyperLinkEdit
            if (tcolumn_type == "HyperLinkEdit")
            {
                RepositoryItemHyperLinkEdit tEdit = new RepositoryItemHyperLinkEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region ImageComboBoxEdit
            if ((tcolumn_type == "ImageComboBoxEdit") ||
                (tcolumn_type == "tImageComboBoxEdit2Button") ||
                (tcolumn_type == "tImageComboBoxEditSEC") ||
                (tcolumn_type == "tImageComboBoxEditSubView")
                )
            {
                RepositoryItemImageComboBox tEdit = new RepositoryItemImageComboBox();
                tEdit.Name = "Column_" + tFieldName;

                RepositoryItemImageComboBox_Fill(tEdit, Row, "", 1); // Tumu = Hayır
                
                if (tcolumn_type == "tImageComboBoxEdit2Button")
                {
                    DevExpress.XtraEditors.Controls.EditorButton tBtn =
                    new DevExpress.XtraEditors.Controls.EditorButton();

                    // Esas Master olan field name
                    tBtn.Caption = Column.FieldName;

                    tEdit.Buttons.Add(tBtn);
                    tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.ImageComboBoxEdit_ButtonClick);
                }

                if (tcolumn_type == "tImageComboBoxEditSEC")
                {
                    DevExpress.XtraEditors.Controls.EditorButton tBtn =
                       new DevExpress.XtraEditors.Controls.EditorButton();

                    tBtn.Caption = "SEÇ";
                    tBtn.Width = 30;
                    tBtn.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Search;

                    tProp_Navigator = "Type:" + v.SearchEngine + ";" + tProp_Navigator;
                    tEdit.AccessibleDescription = tProp_Navigator;

                    tEdit.ReadOnly = true;
                    tEdit.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
                    tEdit.Buttons[0].Visible = false;
                    tEdit.Buttons.Add(tBtn);

                    //tEdit.ButtonClick += new
                    //  DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.ImageComboBoxEdit_ButtonClick);
                    tEdit.ButtonClick += new
                      DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);
                }

                if (tcolumn_type == "tImageComboBoxEditSubView")
                {
                    // treeList
                }
                
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);

                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region ImageEdit
            if (tcolumn_type == "ImageEdit")
            {
                RepositoryItemImageEdit tEdit = new RepositoryItemImageEdit();
                tEdit.Name = "Column_" + tFieldName;
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region ImageListBoxControl
            if (tcolumn_type == "ImageListBoxControl")
            {
                RepositoryItemImageComboBox tEdit = new RepositoryItemImageComboBox();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region LabelControl
            if (tcolumn_type == "LabelControl")
            {
            }
            #endregion

            #region ListBoxControl
            if (tcolumn_type == "ListBoxControl")
            {

            }
            #endregion

            #region LookUpEdit
            if (tcolumn_type == "LookUpEdit")
            {
                RepositoryItemLookUpEdit tEdit = new RepositoryItemLookUpEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region MemoEdit
            if (tcolumn_type == "MemoEdit")
            {
                RepositoryItemMemoEdit tEdit = new RepositoryItemMemoEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.AutoHeight = true;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region MemoExEdit
            if (tcolumn_type == "MemoExEdit")
            {
                RepositoryItemMemoExEdit tEdit = new RepositoryItemMemoExEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region MRUEdit
            if (tcolumn_type == "MRUEdit")
            {
                RepositoryItemMRUEdit tEdit = new RepositoryItemMRUEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region PictureEdit
            if (tcolumn_type == "PictureEdit")
            {
                RepositoryItemPictureEdit tEdit = new RepositoryItemPictureEdit();
                tEdit.Name = "Column_" + tFieldName;
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region PopupContainerControl
            if (tcolumn_type == "PopupContainerControl")
            {
            }
            #endregion

            #region PopupContainerEdit
            if (tcolumn_type == "PopupContainerEdit")
            {
            }
            #endregion

            #region RadioGroup
            if (tcolumn_type == "RadioGroup")
            {
                RepositoryItemRadioGroup tEdit = new RepositoryItemRadioGroup();
                tEdit.Name = "Column_" + tFieldName;

                RepositoryItemRadioGroup_Fill(tEdit, Row, "", 1); // Tumu = hayır
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region RangeTrackBarControl
            if (tcolumn_type == "RangeTrackBarControl")
            {
            }
            #endregion

            #region SpinEdit
            if (tcolumn_type == "SpinEdit")
            {
                RepositoryItemSpinEdit tEdit = new RepositoryItemSpinEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region TextEdit
            if (tcolumn_type == "TextEdit")
            {
                RepositoryItemTextEdit tEdit = new RepositoryItemTextEdit();
                tEdit.Name = "Column_" + tFieldName;
                                
                #region displayformat
                if (t.IsNotNull(tdisplayformat))
                {
                    // 0,  'none'
                    // 1,  'Numeric'
                    // 2,  'DateTime'
                    // 3,  'Custom'
                    Int16 tcmp_format_type = t.Set(Row["CMP_FORMAT_TYPE"].ToString(), Row["LKP_CMP_FORMAT_TYPE"].ToString(), (Int16)0);
                    RepositoryItem_DisplayFormat(tEdit, tdisplayformat, tcmp_format_type);
                }
                #endregion displayformat

                if (tExpressionType > 0)
                    Column.Tag = "EXPRESSION";

                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);

                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region TimeEdit
            if (tcolumn_type == "TimeEdit")
            {
                RepositoryItemTimeEdit tEdit = new RepositoryItemTimeEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region TimeSpanEdit
            if (tcolumn_type == "TimeSpanEdit")
            {
                RepositoryItemTimeSpanEdit tEdit = new RepositoryItemTimeSpanEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.AllowEditDays = false;

                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);

                Column.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.DateTime;
                Column.ColumnEdit = tEdit;
            }
            #endregion
            
            #region TrackBarControl
            if (tcolumn_type == "TrackBarControl")
            {
                RepositoryItemTrackBar tEdit = new RepositoryItemTrackBar();
                tEdit.Name = "Column_" + tFieldName;
                Column.ColumnEdit = tEdit;
            }
            #endregion

        }

        #endregion
        
        #region Tile_ColumnEdit

        public void Tile_ColumnEdit(DataRow Row, DevExpress.XtraGrid.Columns.TileViewColumn Column, string tcolumn_type)
        {
            tToolBox t = new tToolBox();
            tEvents ev = new tEvents();

            string tTableName = Row["LKP_TABLE_NAME"].ToString();
            string tFieldName = Row["LKP_FIELD_NAME"].ToString();
            string tExpression = Row["LKP_PROP_EXPRESSION"].ToString();
            // formülleri çalıştırmak için etkisi varmı
            // 0 = yok
            // 1 = true
            Int16 tExpressionType = t.myInt16(Row["LKP_EXPRESSION_TYPE"].ToString());
            float font_size = t.Set(Row["CMP_FONT_SIZE"].ToString(), "", (float)0);
            string tdisplayformat = t.Set(Row["CMP_DISPLAY_FORMAT"].ToString(), Row["LKP_CMP_DISPLAY_FORMAT"].ToString(), "");
            string tProp_Navigator = t.Set(Row["PROP_NAVIGATOR"].ToString(), "", "");

            #region tPropertiesEdit
            if (tcolumn_type == "tPropertiesEdit")
            {
                RepositoryItemButtonEdit tEdit = new RepositoryItemButtonEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                tEdit.KeyDown += new KeyEventHandler(ev.buttonEdit_KeyDown);
                tEdit.KeyPress += new KeyPressEventHandler(ev.buttonEdit_KeyPress);
                tEdit.KeyUp += new KeyEventHandler(ev.buttonEdit_KeyUp);
                tEdit.Leave += new EventHandler(ev.buttonEdit_Leave);
                tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);

                Int16 width = t.Set(Row["LKP_CMP_WIDTH"].ToString(), "", (Int16)200);

                string s = string.Empty;
                t.MyProperties_Set(ref s, "Type", v.Properties);
                t.MyProperties_Set(ref s, "TableName", tTableName);
                t.MyProperties_Set(ref s, "FieldName", tFieldName);
                t.MyProperties_Set(ref s, "Width", width.ToString());

                tEdit.AccessibleDescription = s;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);

                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region tPropertiesPlusEdit
            if (tcolumn_type == "tPropertiesPlusEdit")
            {
                RepositoryItemButtonEdit tEdit = new RepositoryItemButtonEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                tEdit.KeyDown += new KeyEventHandler(ev.buttonEdit_KeyDown);
                tEdit.KeyPress += new KeyPressEventHandler(ev.buttonEdit_KeyPress);
                tEdit.KeyUp += new KeyEventHandler(ev.buttonEdit_KeyUp);
                tEdit.Leave += new EventHandler(ev.buttonEdit_Leave);
                tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);

                Int16 width = t.Set(Row["LKP_CMP_WIDTH"].ToString(), "", (Int16)200);

                string s = string.Empty;
                t.MyProperties_Set(ref s, "Type", v.PropertiesPlus);
                t.MyProperties_Set(ref s, "TableName", tTableName);
                t.MyProperties_Set(ref s, "FieldName", tFieldName);
                t.MyProperties_Set(ref s, "Width", width.ToString());

                tEdit.AccessibleDescription = s;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);

                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region ButtonEdit
            if ((tcolumn_type == "ButtonEdit") ||
                    (tcolumn_type == "tSearchEdit"))
            {
                RepositoryItemButtonEdit tEdit = new RepositoryItemButtonEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.Enter += new EventHandler(ev.buttonEdit_Enter);
                tEdit.KeyDown += new KeyEventHandler(ev.buttonEdit_KeyDown);
                tEdit.KeyPress += new KeyPressEventHandler(ev.buttonEdit_KeyPress);
                tEdit.KeyUp += new KeyEventHandler(ev.buttonEdit_KeyUp);
                tEdit.Leave += new EventHandler(ev.buttonEdit_Leave);
                tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.buttonEdit_ButtonClick);

                if (tFieldName == "LKP_KOMUT")
                {
                    tEdit.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.OK;
                    tEdit.Buttons[0].Width = 40; // Column.Width / 2;
                    tEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                }
                
                #region tSearchEdit
                if (tcolumn_type == "tSearchEdit")
                {
                    tProp_Navigator = "Type:" + v.SearchEngine + ";" + tProp_Navigator;
                    tEdit.AccessibleDescription = tProp_Navigator;
                    tEdit.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Search;
                }
                #endregion tSearchEdit


                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region CalcEdit
            if (tcolumn_type == "CalcEdit")
            {
                RepositoryItemCalcEdit tEdit = new RepositoryItemCalcEdit();
                tEdit.Name = "Column_" + tFieldName;

                if (tExpressionType > 0)
                    Column.Tag = "EXPRESSION";
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region CheckButton
            if (tcolumn_type == "CheckButton")
            {
            }
            #endregion

            #region CheckedComboBoxEdit
            if (tcolumn_type == "CheckedComboBoxEdit")
            {
                RepositoryItemCheckedComboBoxEdit tEdit = new RepositoryItemCheckedComboBoxEdit();
                tEdit.Name = "Column_" + tFieldName;

                RepositoryItemCheckedComboBoxEdit_Fill(tEdit, Row);
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region CheckEdit
            if (tcolumn_type == "CheckEdit")
            {
                RepositoryItemCheckEdit tEdit = new RepositoryItemCheckEdit();
                tEdit.Name = "Column_" + tFieldName;

                byte ftype = t.Set(Row["LKP_FIELD_TYPE"].ToString(), "", (byte)0);

                if (ftype == 52) // Int16
                {
                    tEdit.BeginInit();
                    tEdit.ValueChecked = ((short)(1));
                    tEdit.ValueUnchecked = ((short)(0));
                    tEdit.EndInit();
                }

                if (ftype == 56) // int
                {
                    tEdit.BeginInit();
                    tEdit.ValueChecked = 1;
                    tEdit.ValueUnchecked = 0;
                    tEdit.EndInit();
                }
                if (ftype == 167) // varchar
                {
                    tEdit.BeginInit();
                    tEdit.ValueUnchecked = "";
                    tEdit.ValueChecked = "E";
                    tEdit.ValueUnchecked = "H";
                    tEdit.EndInit();
                }

                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region CheckedListBoxControl
            if (tcolumn_type == "CheckedListBoxControl")
            {
            }
            #endregion

            #region ComboBoxEdit
            if (tcolumn_type == "ComboBoxEdit")
            {
                RepositoryItemComboBox tEdit = new RepositoryItemComboBox();
                tEdit.Name = "Column_" + tFieldName;

                RepositoryItemComboBox_Fill(tEdit, Row, "", 1); // Tumu = hayır
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region DateEdit
            if (tcolumn_type == "DateEdit")
            {
                RepositoryItemDateEdit tEdit = new RepositoryItemDateEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region HyperLinkEdit
            if (tcolumn_type == "HyperLinkEdit")
            {
                RepositoryItemHyperLinkEdit tEdit = new RepositoryItemHyperLinkEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region ImageComboBoxEdit
            if ((tcolumn_type == "ImageComboBoxEdit") ||
                (tcolumn_type == "tImageComboBoxEdit2Button"))
            {
                RepositoryItemImageComboBox tEdit = new RepositoryItemImageComboBox();
                tEdit.Name = "Column_" + tFieldName;

                RepositoryItemImageComboBox_Fill(tEdit, Row, "", 1); // Tumu = Hayır

                if (tcolumn_type == "tImageComboBoxEdit2Button")
                {
                    DevExpress.XtraEditors.Controls.EditorButton tBtn =
                    new DevExpress.XtraEditors.Controls.EditorButton();

                    // Esas Master olan field name
                    tBtn.Caption = Column.FieldName;

                    tEdit.Buttons.Add(tBtn);
                    tEdit.ButtonClick += new
                     DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ev.ImageComboBoxEdit_ButtonClick);
                }

                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);

                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region ImageEdit
            if (tcolumn_type == "ImageEdit")
            {
                RepositoryItemImageEdit tEdit = new RepositoryItemImageEdit();
                tEdit.Name = "Column_" + tFieldName;
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region ImageListBoxControl
            if (tcolumn_type == "ImageListBoxControl")
            {
                RepositoryItemImageComboBox tEdit = new RepositoryItemImageComboBox();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region LabelControl
            if (tcolumn_type == "LabelControl")
            {
            }
            #endregion

            #region ListBoxControl
            if (tcolumn_type == "ListBoxControl")
            {

            }
            #endregion

            #region LookUpEdit
            if (tcolumn_type == "LookUpEdit")
            {
                RepositoryItemLookUpEdit tEdit = new RepositoryItemLookUpEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region MemoEdit
            if (tcolumn_type == "MemoEdit")
            {
                RepositoryItemMemoEdit tEdit = new RepositoryItemMemoEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.AutoHeight = true;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region MemoExEdit
            if (tcolumn_type == "MemoExEdit")
            {
                RepositoryItemMemoExEdit tEdit = new RepositoryItemMemoExEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region MRUEdit
            if (tcolumn_type == "MRUEdit")
            {
                RepositoryItemMRUEdit tEdit = new RepositoryItemMRUEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region PictureEdit
            if (tcolumn_type == "PictureEdit")
            {
                RepositoryItemPictureEdit tEdit = new RepositoryItemPictureEdit();
                tEdit.Name = "Column_" + tFieldName;
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region PopupContainerControl
            if (tcolumn_type == "PopupContainerControl")
            {
            }
            #endregion

            #region PopupContainerEdit
            if (tcolumn_type == "PopupContainerEdit")
            {
            }
            #endregion

            #region RadioGroup
            if (tcolumn_type == "RadioGroup")
            {
                RepositoryItemRadioGroup tEdit = new RepositoryItemRadioGroup();
                tEdit.Name = "Column_" + tFieldName;

                RepositoryItemRadioGroup_Fill(tEdit, Row, "", 1); // Tumu = hayır
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region RangeTrackBarControl
            if (tcolumn_type == "RangeTrackBarControl")
            {
            }
            #endregion

            #region SpinEdit
            if (tcolumn_type == "SpinEdit")
            {
                RepositoryItemSpinEdit tEdit = new RepositoryItemSpinEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region TextEdit
            if (tcolumn_type == "TextEdit")
            {
                RepositoryItemTextEdit tEdit = new RepositoryItemTextEdit();
                tEdit.Name = "Column_" + tFieldName;

                #region displayformat
                if (t.IsNotNull(tdisplayformat))
                {
                    // 0,  'none'
                    // 1,  'Numeric'
                    // 2,  'DateTime'
                    // 3,  'Custom'
                    Int16 tcmp_format_type = t.Set(Row["CMP_FORMAT_TYPE"].ToString(), Row["LKP_CMP_FORMAT_TYPE"].ToString(), (Int16)0);
                    RepositoryItem_DisplayFormat(tEdit, tdisplayformat, tcmp_format_type);
                }
                #endregion displayformat

                if (tExpressionType > 0)
                    Column.Tag = "EXPRESSION";

                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);

                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region TimeEdit
            if (tcolumn_type == "TimeEdit")
            {
                RepositoryItemTimeEdit tEdit = new RepositoryItemTimeEdit();
                tEdit.Name = "Column_" + tFieldName;
                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region TimeSpanEdit
            if (tcolumn_type == "TimeSpanEdit")
            {
                RepositoryItemTimeSpanEdit tEdit = new RepositoryItemTimeSpanEdit();
                tEdit.Name = "Column_" + tFieldName;
                tEdit.AllowEditDays = false;

                if (font_size != 0)
                    tEdit.Appearance.Font = new Font("Tahoma", font_size);

                Column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                Column.ColumnEdit = tEdit;
            }
            #endregion

            #region TrackBarControl
            if (tcolumn_type == "TrackBarControl")
            {
                RepositoryItemTrackBar tEdit = new RepositoryItemTrackBar();
                tEdit.Name = "Column_" + tFieldName;
                Column.ColumnEdit = tEdit;
            }
            #endregion

        }

        #endregion
        
        #region tUserPictureControl

        public Control Create_UserPictureControl(PictureEdit pictureEdit1, string FieldName, string FormName)
        {
            TableLayoutPanel tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ////PictureEdit pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            //PopupContainerEdit popupContainerEdit1 = new DevExpress.XtraEditors.PopupContainerEdit();
            //PopupContainerControl popupContainerControl1 = new DevExpress.XtraEditors.PopupContainerControl();
            SimpleButton btn_tPictures = new DevExpress.XtraEditors.SimpleButton();


            /*
            //WebCam_Capture.WebCamCapture webCamCapture1 = new WebCam_Capture.WebCamCapture();//
            //v.webCamCapture1 = new WebCam_Capture.WebCamCapture();
            CheckEdit checkEdit_Tarayici = new DevExpress.XtraEditors.CheckEdit();
            SimpleButton btn_Tarayici = new DevExpress.XtraEditors.SimpleButton();
            SimpleButton btn_Dosya = new DevExpress.XtraEditors.SimpleButton();
            //SimpleButton btn_WebCam = new DevExpress.XtraEditors.SimpleButton();
            SimpleButton btn_Delete = new DevExpress.XtraEditors.SimpleButton();
            SimpleButton btn_SaveAs = new DevExpress.XtraEditors.SimpleButton();
            SimpleButton btn_Save = new DevExpress.XtraEditors.SimpleButton();
            ZoomTrackBarControl zoomTrackBarControl1 = new DevExpress.XtraEditors.ZoomTrackBarControl();
            LabelControl labelControl_PictureSize = new DevExpress.XtraEditors.LabelControl();
            ButtonEdit btn_Quality = new DevExpress.XtraEditors.ButtonEdit();
            ButtonEdit btn_Compress = new DevExpress.XtraEditors.ButtonEdit();
            ButtonEdit btn_AutoCorps = new DevExpress.XtraEditors.ButtonEdit();
            SimpleButton btn_Original = new DevExpress.XtraEditors.SimpleButton();

            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            */

            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(pictureEdit1.Properties)).BeginInit();
            //((System.ComponentModel.ISupportInitialize)(popupContainerEdit1.Properties)).BeginInit();
            //((System.ComponentModel.ISupportInitialize)(popupContainerControl1)).BeginInit();
            //popupContainerControl1.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(zoomTrackBarControl1)).BeginInit();
            //((System.ComponentModel.ISupportInitialize)(zoomTrackBarControl1.Properties)).BeginInit();
            //((System.ComponentModel.ISupportInitialize)(checkEdit_Tarayici.Properties)).BeginInit();
            //SuspendLayout();

            // 
            // tableLayoutPanel1 (silme)
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            //tableLayoutPanel1.Controls.Add(popupContainerEdit1, 0, 1);
            tableLayoutPanel1.Controls.Add(btn_tPictures, 0, 1);
            tableLayoutPanel1.Controls.Add(pictureEdit1, 0, 0);
            tableLayoutPanel1.Location = new System.Drawing.Point(31, 60);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            tableLayoutPanel1.Size = new System.Drawing.Size(130, 175);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // pictureEdit1
            // 
            pictureEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            pictureEdit1.Location = new System.Drawing.Point(3, 3);
            //pictureEdit1.Name = "pictureEdit1";
            pictureEdit1.Properties.ShowScrollBars = true;
            pictureEdit1.Properties.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.True;
            pictureEdit1.Size = new System.Drawing.Size(124, 141);
            pictureEdit1.TabIndex = 0;
            //pictureEdit1.AccessibleName = FieldName;
            pictureEdit1.MouseMove += new System.Windows.Forms.MouseEventHandler(pictureEdit1_MouseMove);
            pictureEdit1.MouseDown += new System.Windows.Forms.MouseEventHandler(pictureEdit1_MouseDown);
            pictureEdit1.Paint += new System.Windows.Forms.PaintEventHandler(pictureEdit1_Paint);
            pictureEdit1.MouseUp += new System.Windows.Forms.MouseEventHandler(pictureEdit1_MouseUp);
            // 
            // btn_tPictures
            // 
            btn_tPictures.Dock = DockStyle.Right;
            btn_tPictures.Location = new System.Drawing.Point(1, 1);
            btn_tPictures.Name = "btn_tPictures";
            btn_tPictures.Size = new System.Drawing.Size(200, 23);
            btn_tPictures.TabIndex = 1;
            btn_tPictures.Text = "Resim Ekleme / Düzenleme";
            btn_tPictures.AccessibleName = FieldName;
            btn_tPictures.AccessibleDescription = FormName;
            btn_tPictures.Click += new System.EventHandler(btn_tPictures_Click);
            // 
            // popupContainerEdit1
            // 
            ////popupContainerEdit1.EditValue = "Resim İşlemleri";
            ////popupContainerEdit1.Location = new System.Drawing.Point(3, 150);
            ////popupContainerEdit1.Name = "popupContainerEdit1";
            ////popupContainerEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            ////new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis)});
            ////popupContainerEdit1.Properties.PopupControl = popupContainerControl1;
            ////popupContainerEdit1.Size = new System.Drawing.Size(124, 20);
            ////popupContainerEdit1.TabIndex = 1;
            ////popupContainerEdit1.Dock = DockStyle.Fill;
            ////popupContainerEdit1.AccessibleName = FieldName;
            ////popupContainerEdit1.AccessibleDescription = FormName;
            ////popupContainerEdit1.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(popupContainerEdit1_ButtonClick);
            // 
            // popupContainerControl1
            // 
            ////popupContainerControl1.Controls.Add(checkEdit_Tarayici);
            ////popupContainerControl1.Controls.Add(btn_Tarayici);
            ////popupContainerControl1.Controls.Add(btn_Dosya);
            //////popupContainerControl1.Controls.Add(btn_WebCam);
            ////popupContainerControl1.Controls.Add(btn_Delete);
            ////popupContainerControl1.Controls.Add(btn_SaveAs);
            ////popupContainerControl1.Controls.Add(btn_Save);
            ////popupContainerControl1.Controls.Add(zoomTrackBarControl1);
            ////popupContainerControl1.Controls.Add(labelControl_PictureSize);
            ////popupContainerControl1.Controls.Add(btn_AutoCorps);
            ////popupContainerControl1.Controls.Add(btn_Quality);
            ////popupContainerControl1.Controls.Add(btn_Compress);
            ////popupContainerControl1.Controls.Add(btn_Original);
            ////popupContainerControl1.Location = new System.Drawing.Point(31, 241);
            ////popupContainerControl1.Name = "popupContainerControl1";
            ////popupContainerControl1.Size = new System.Drawing.Size(322, 221);
            ////popupContainerControl1.TabIndex = 1;
            // 
            // checkEdit_Tarayici
            // 
            ////checkEdit_Tarayici.Location = new System.Drawing.Point(4, 7);
            ////checkEdit_Tarayici.Name = "checkEdit_Tarayici";
            ////checkEdit_Tarayici.Properties.Caption = "Tarayıcı Listesi";
            ////checkEdit_Tarayici.Size = new System.Drawing.Size(93, 19);
            ////checkEdit_Tarayici.TabIndex = 0;
            // 
            // btn_Tarayici
            // 
            ////btn_Tarayici.Location = new System.Drawing.Point(4, 32);
            ////btn_Tarayici.Name = "btn_Tarayici";
            ////btn_Tarayici.Size = new System.Drawing.Size(100, 23);
            ////btn_Tarayici.TabIndex = 1;
            ////btn_Tarayici.Text = "Tarayıcıdan Al";
            ////btn_Tarayici.AccessibleName = FieldName;
            ////btn_Tarayici.AccessibleDescription = FormName;
            ////btn_Tarayici.Click += new System.EventHandler(btn_Tarayici_Click);
            // 
            // btn_Dosya
            // 
            ////btn_Dosya.Location = new System.Drawing.Point(110, 32);
            ////btn_Dosya.Name = "btn_Dosya";
            ////btn_Dosya.Size = new System.Drawing.Size(100, 23);
            ////btn_Dosya.TabIndex = 2;
            ////btn_Dosya.Text = "Dosyadan Al";
            ////btn_Dosya.AccessibleName = FieldName;
            ////btn_Dosya.AccessibleDescription = FormName;
            ////btn_Dosya.Click += new System.EventHandler(btn_Dosya_Click);
            // 
            // btn_WebCam
            // 
            //btn_WebCam.Location = new System.Drawing.Point(215, 32);
            //btn_WebCam.Name = "btn_WebCam";
            //btn_WebCam.Size = new System.Drawing.Size(100, 23);
            //btn_WebCam.TabIndex = 3;
            //btn_WebCam.Text = "WebCam den Al";
            //btn_WebCam.AccessibleName = FieldName;
            //btn_WebCam.AccessibleDescription = FormName;
            //btn_WebCam.Click += new System.EventHandler(btn_WebCam_Click);
            // 
            // btn_Delete
            // 
            ////btn_Delete.Location = new System.Drawing.Point(4, 64);
            ////btn_Delete.Name = "btn_Delete";
            ////btn_Delete.Size = new System.Drawing.Size(100, 25);
            ////btn_Delete.TabIndex = 4;
            ////btn_Delete.Text = "Resmi SİL";
            ////btn_Delete.AccessibleName = FieldName;
            ////btn_Delete.AccessibleDescription = FormName;
            ////btn_Delete.Click += new System.EventHandler(btn_Delete_Click);
            // 
            // btn_SaveAs
            // 
            ////btn_SaveAs.Location = new System.Drawing.Point(110, 64);
            ////btn_SaveAs.Name = "btn_SaveAs";
            ////btn_SaveAs.Size = new System.Drawing.Size(100, 23);
            ////btn_SaveAs.TabIndex = 5;
            ////btn_SaveAs.Text = "Dışarı Kaydet";
            ////btn_SaveAs.AccessibleName = FieldName;
            ////btn_SaveAs.AccessibleDescription = FormName;
            ////btn_SaveAs.Click += new System.EventHandler(btn_SaveAs_Click);
            // 
            // btn_Save
            // 
            ////btn_Save.Location = new System.Drawing.Point(215, 64);
            ////btn_Save.Name = "btn_Save";
            ////btn_Save.Size = new System.Drawing.Size(100, 23);
            ////btn_Save.TabIndex = 6;
            ////btn_Save.Text = "Kaydet";
            ////btn_Save.Enabled = false;
            ////btn_Save.AccessibleName = FieldName;
            ////btn_Save.AccessibleDescription = FormName;
            ////btn_Save.Click += new System.EventHandler(btn_Save_Click);
            // 
            // zoomTrackBarControl1
            // 
            ////zoomTrackBarControl1.EditValue = 100;
            ////zoomTrackBarControl1.Location = new System.Drawing.Point(10, 92);
            ////zoomTrackBarControl1.Name = "zoomTrackBarControl1";
            ////zoomTrackBarControl1.Properties.LargeChange = 25;
            ////zoomTrackBarControl1.Properties.Maximum = 400;
            ////zoomTrackBarControl1.Properties.Middle = 5;
            ////zoomTrackBarControl1.Properties.Minimum = 10;
            ////zoomTrackBarControl1.Properties.ScrollThumbStyle = DevExpress.XtraEditors.Repository.ScrollThumbStyle.ArrowDownRight;
            ////zoomTrackBarControl1.Size = new System.Drawing.Size(298, 23);
            ////zoomTrackBarControl1.TabIndex = 7;
            ////zoomTrackBarControl1.Value = 100;
            ////zoomTrackBarControl1.AccessibleName = FieldName;
            ////zoomTrackBarControl1.AccessibleDescription = FormName;
            ////zoomTrackBarControl1.EditValueChanged += new System.EventHandler(zoomTrackBarControl1_EditValueChanged);
            // 
            // labelControl_PictureSize
            // 
            ////labelControl_PictureSize.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            ////labelControl_PictureSize.Location = new System.Drawing.Point(6, 121);
            ////labelControl_PictureSize.Name = "labelControl_PictureSize";
            ////labelControl_PictureSize.Size = new System.Drawing.Size(309, 35);
            ////labelControl_PictureSize.TabIndex = 8;
            ////labelControl_PictureSize.Text = "Lütfen yükleyeceğiniz resmin JPG formatında \r\nolmasına dikkat edin.";
            //labelControl_PictureSize.Text = "Resim Boyutu : 125 kb ( 125000 byte ) \r\nDikkat resmin boyutunu küçültmeniz gereki" +
            //"yor.";
            // 
            // btn_AutoCorps
            // 
            ////btn_AutoCorps.EditValue = "10  pixel";
            ////btn_AutoCorps.Location = new System.Drawing.Point(6, 166);
            ////btn_AutoCorps.Name = "btn_AutoCorps";
            ////btn_AutoCorps.Properties.Buttons.Clear();
            ////btn_AutoCorps.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            ////new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinDown),
            ////new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinUp),
            ////new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "Oto. Kırp", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, "", null, null, true)});
            ////btn_AutoCorps.Size = new System.Drawing.Size(140, 20);
            ////btn_AutoCorps.TabIndex = 9;
            ////btn_AutoCorps.AccessibleName = FieldName;
            ////btn_AutoCorps.AccessibleDescription = FormName;
            ////btn_AutoCorps.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(btn_AutoCorps_ButtonClick);
            ////btn_AutoCorps.Enabled = false;
            // 
            // btn_Compress
            // 
            ////btn_Compress.EditValue = "10  pixel";
            ////btn_Compress.Location = new System.Drawing.Point(183, 165);
            ////btn_Compress.Name = "btn_Compress";
            ////btn_Compress.Properties.Buttons.Clear();
            ////btn_Compress.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            ////new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinDown),
            ////new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinUp),
            ////new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "Sıkıştır", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject3, "", null, null, true)});
            ////btn_Compress.Size = new System.Drawing.Size(132, 20);
            ////btn_Compress.TabIndex = 10;
            ////btn_Compress.AccessibleName = FieldName;
            ////btn_Compress.AccessibleDescription = FormName;
            ////btn_Compress.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(btn_Compress_ButtonClick);
            ////btn_Compress.Enabled = false;
            // 
            // btn_Quality
            // 
            ////btn_Quality.EditValue = "50  %";
            ////btn_Quality.Location = new System.Drawing.Point(183, 191);
            ////btn_Quality.Name = "btn_Quality";
            ////btn_Quality.Properties.Buttons.Clear();
            ////btn_Quality.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            ////new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinDown),
            ////new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinUp),
            ////new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "Kaliteyi Düşür", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject2, "", null, null, true)});
            ////btn_Quality.Size = new System.Drawing.Size(132, 20);
            ////btn_Quality.TabIndex = 11;
            ////btn_Quality.AccessibleName = FieldName;
            ////btn_Quality.AccessibleDescription = FormName;
            ////btn_Quality.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(btn_Quality_ButtonClick);
            ////btn_Quality.Enabled = false;
            // 
            // btn_Original
            // 
            ////btn_Original.Location = new System.Drawing.Point(6, 192);
            ////btn_Original.Name = "btn_Original";
            ////btn_Original.Size = new System.Drawing.Size(140, 20);
            ////btn_Original.TabIndex = 12;
            ////btn_Original.Text = "Orjinali Yükle";
            ////btn_Original.AccessibleName = FieldName;
            ////btn_Original.AccessibleDescription = FormName;
            ////btn_Original.Click += new System.EventHandler(btn_Orginal_Click);

            // 
            // webCamCapture1
            // 
            //v.webCamCapture1.CaptureHeight = 240;
            //v.webCamCapture1.CaptureWidth = 320;
            //v.webCamCapture1.FrameNumber = ((ulong)(0ul));
            //v.webCamCapture1.Location = new System.Drawing.Point(0, 0);
            //v.webCamCapture1.Name = "WebCamCapture";
            //v.webCamCapture1.Size = new System.Drawing.Size(342, 252);
            //v.webCamCapture1.TabIndex = 0;
            //v.webCamCapture1.TimeToCapture_milliseconds = 100;
            //v.webCamCapture1.ImageCaptured += new WebCam_Capture.WebCamCapture.WebCamEventHandler(webCamCapture1_ImageCaptured);
            //v.webCamCapture1.AccessibleName = FieldName;
            //v.webCamCapture1.AccessibleDescription = FormName;
            //
            //
            //
            tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(pictureEdit1.Properties)).EndInit();
            ////((System.ComponentModel.ISupportInitialize)(popupContainerEdit1.Properties)).EndInit();
            ////((System.ComponentModel.ISupportInitialize)(popupContainerControl1)).EndInit();
            ////popupContainerControl1.ResumeLayout(false);
            ////popupContainerControl1.PerformLayout();
            //flowLayoutPanel1.ResumeLayout(false);
            ////((System.ComponentModel.ISupportInitialize)(zoomTrackBarControl1.Properties)).EndInit();
            ////((System.ComponentModel.ISupportInitialize)(zoomTrackBarControl1)).EndInit();
            ////((System.ComponentModel.ISupportInitialize)(checkEdit_Tarayici.Properties)).EndInit();

            return tableLayoutPanel1;
        }
        
        private void popupContainerEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            tToolBox t = new tToolBox();

            string FieldName = ((DevExpress.XtraEditors.PopupContainerEdit)sender).AccessibleName;
            string FormName = ((DevExpress.XtraEditors.PopupContainerEdit)sender).AccessibleDescription;

            Control pictureEdit1 = myPictureEdit_Get(FormName, FieldName);

            Control label_PictureSize = myPictureSize_Get(FormName, FieldName);

            if (pictureEdit1 != null)
            {

                if (label_PictureSize != null)
                
                {
                    if (((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image != null)
                    {
                        ((DevExpress.XtraEditors.LabelControl)label_PictureSize).Text = "Resim Formatı : " +
                           ((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image.PixelFormat.ToString();
                    }
                    else
                    {
                        ((DevExpress.XtraEditors.LabelControl)label_PictureSize).Text = "Yükleyeceğiniz resmin formatı JPG olmalı...";
                    }
                }
            }
            
            /*
             * buradaki hesap yanlış çıktı
            
            //Form tForm = Application.OpenForms[FormName];
            
            Control pictureEdit1 = myPictureEdit_Get(FormName, FieldName);

            Control label_PictureSize = myPictureSize_Get(FormName, FieldName);

            if (pictureEdit1 != null)
            {
                MemoryStream mstr = new MemoryStream();
                ((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image.Save(mstr, System.Drawing.Imaging.ImageFormat.Jpeg);

                long sayac = mstr.Capacity;

                //StreamReader sr = new StreamReader(mstr);
                
                //BinaryReader bReader = new BinaryReader(sr);
                
                //string temp = sr.ReadToEnd();
                //long sayac = temp.Length; //sr.ReadByte();
                
                v.con_Images_Length = sayac;

                if (label_PictureSize != null)
                {
                    ((DevExpress.XtraEditors.LabelControl)label_PictureSize).Text = "Resim Boyutu : "
                        + Convert.ToString(v.con_Images_Length / 1000) + " kb. ( " + v.con_Images_Length.ToString() + " byte )";
                }

                //byte[] byteResim = bReader.ReadBytes((int)sayac);
                //v.con_Images_Length = byteResim.Length;

            }
            */
        }

        private void btn_tPictures_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();
            tForms fr = new tForms();
            Form tNewForm = null;
            tNewForm = fr.Get_Form("ms_Pictures");
            t.ChildForm_View(tNewForm, Application.OpenForms[0], FormWindowState.Maximized, v.con_FormLoadValue); //myFormLoadValue);

        }

        private void btn_Dosya_Click(object sender, EventArgs e)
        {
            //Bilgiler kaydedilirken gelen resmin neye dair olduğunu anlayıp ona göre kayıt gerekli.
            //Bundan dolayı tip bilgisi enum olarak saklanıyor.
            //HelperMethod.IslemTur = HelperMethod.Tur.Dosya;

            //Resim seçilir, resmin yolu pbx1 in tag ine atılır.
            OpenFileDialog fdialog = new OpenFileDialog();//Resmi kullanıcıya seçtirmek için bir open file dialog oluşturuyoruz.
            fdialog.Filter = "Pictures|*.jpg";//Seçilecek dosyanın tipi sadece resim olacağı için resim dosya tiplerini filter olarak belirliyoruz.
            //fdialog.InitialDirectory = "C://"; //Kullanıcının resimleri seçeceği dizinin C sabit diski olduğunu belirtiyoruz.

            if (DialogResult.OK == fdialog.ShowDialog())// Kullanıcı bir resim seçmiş mi kontrol ediyoruz.
            {
                // şimdilik gerek kalmadı
                //v.con_Images_Name = fdialog.SafeFileName; // RESİM ADI.
                string ImagesPath = fdialog.FileName; ;   // Seçilen resme ait tam yol.
                
                string FieldName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;
                string FormName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDescription;

                // yeni dosya/Image yükleniyor
                v.con_Images_MasterPath = "";      //(orjinal dosya adı saklanıyor)
                //v.con_Image_Original = null;     //(orjinal resim image nesnesi üzerine alınıyor)

                Preparing_ImageSave(FormName, FieldName, ImagesPath);
            }

            fdialog.Dispose();
        }
        
        private void btn_Tarayici_Click(object sender, EventArgs e)
        {
            try
            {
                tToolBox t = new tToolBox();

                //Tarayıcı kontrolü, bağlı değilse catch e düşer.

                ImageFile ImgFile;
                
                WIA.CommonDialog dialog = new WIA.CommonDialog();
                
                string FieldName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;
                string FormName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDescription;

                Form tForm = Application.OpenForms[FormName];

                string[] controls = new string[] { };

                Control popupContainerEdit1 = t.Find_Control(tForm, "popupContainerEdit1", "", controls);
                
                if (popupContainerEdit1 != null)
                {
                    Control checkEdit_Tarayici = t.Find_PopupContainerEdit(((PopupContainerEdit)popupContainerEdit1), "checkEdit_Tarayici");

                    if (checkEdit_Tarayici != null)
                    {
                        //Tarayıcı bağlı değil ise img alınırken hataya düşüyor, catch ile yakalanıp tarayıcı olmadığı basılıyor.
                        if (((DevExpress.XtraEditors.CheckEdit)checkEdit_Tarayici).Checked)
                        {
                            //Her girişte Cihaz seçimi yapılacaksa.
                            ImgFile = dialog.ShowAcquireImage(WiaDeviceType.ScannerDeviceType, WiaImageIntent.ColorIntent, WiaImageBias.MinimizeSize, "{00000000-0000-0000-0000-000000000000}", true, true, false);
                        }
                        else
                        {
                            //cihaz seçimi yapılmaksızın seçili cihaz ile sürekli devam edilecekse.
                            ImgFile = dialog.ShowAcquireImage(WiaDeviceType.ScannerDeviceType, WiaImageIntent.ColorIntent, WiaImageBias.MinimizeSize, "{00000000-0000-0000-0000-000000000000}", false, true, false);
                        }

                        try
                        {
                            // exenin bulunduğu path altına images isimli path içine guid isimli dosya hazırlanıyor
                            string Images_Path = t.Find_Path("images") + myFileGuidName + ".jpg";

                            //Tarama bölümüne girip iptal dedikten sonra işlemler devam ediyor ve hataya sebebiyet veriyor. Kontrol altına alındı.img.savefile da patlıyor iptal dendiğinde.
                            ImgFile.SaveFile(Images_Path);

                            // yeni resmin orjinal halide hafızaya alınsın
                            v.con_Images_MasterPath = "";
                            //v.con_Image_Original = null;

                            // Kaydet butonunu kayıt için diğer işlemleri hazırla
                            Preparing_ImageSave(FormName, FieldName, Images_Path);
                        }
                        catch (Exception)
                        {
                            //Hatayı alıp programın devam etmesi adına herhangi birşey girilmedi.
                        }

                    } // if (checkEdit_Tarayici

                } // if (popupContainerEdit1

            }
            catch (Exception)
            {
                MessageBox.Show("Bilgisayara bağlı bir tarayıcı bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
                
        private void btn_WebCam_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            string FieldName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;
            string FormName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDescription;

            Form tForm = Application.OpenForms[FormName];

            if (((DevExpress.XtraEditors.SimpleButton)sender).Text == "WebCam den Al")
            {
                ((DevExpress.XtraEditors.SimpleButton)sender).Text = "Fotoğraf Çek";

                //2 kez start yapılmazsa çalışmıyor.
                v.webCamCapture1.Start(0);
                v.webCamCapture1.Start(0);
 
                Application.DoEvents();

                btn_SaveEnabledChance(tForm, false);

                //SendMessage(v.webCamCapture1, WM_CAP_DISCONNECT, 0, 0);
            }
            else
            {
                ((DevExpress.XtraEditors.SimpleButton)sender).Text = "WebCam den Al";
                
                try
                {
                    v.webCamCapture1.Stop();
                    v.webCamCapture1.Stop();

                    Application.DoEvents();

                    Control pictureEdit1 = myPictureEdit_Get(FormName, FieldName);

                     if (pictureEdit1 != null)
                     {
                         // exenin bulunduğu path altına images isimli path içine guid isimli dosya hazırlanıyor
                         string Images_Path = t.Find_Path("images") + myFileGuidName + ".jpg";

                         ((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image.Save(Images_Path);

                         // yeni resmin orjinal halide hafızaya alınsın
                         //v.con_Image_Original = null;
                         v.con_Images_MasterPath = "";

                         // Kaydet butonunu kayıt için diğer işlemleri hazırla
                         Preparing_ImageSave(FormName, FieldName, Images_Path);

                         btn_SaveEnabledChance(tForm, true);
                     }
                }
                catch (Exception)
                {
                    //Hatayı alıp programın devam etmesi adına herhangi birşey girilmedi.
                }

                //v.webCamCapture1 = null;
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();
            
            string soru = "Resim silinecek, Onaylıyor musunuz ?" + v.ENTER2 + "Not : Sildikten sonra kaydetmeyi unutmayınız ...";
            DialogResult cevap = t.mySoru(soru);

            if (DialogResult.Yes != cevap)
            {
                return;
            }
                        
            string FieldName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;
            string FormName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDescription;

            Form tForm = Application.OpenForms[FormName];

            Control pictureEdit1 = myPictureEdit_Get(FormName, FieldName);
            
            if (pictureEdit1 != null)
            {
                v.con_Images = null;
                v.con_Images_FieldName = FieldName;
                ((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image = null;

                btn_SaveEnabledChance(tForm, true);
            }
        }

        private void btn_SaveAs_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            string FieldName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;
            string FormName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDescription;

            Control pictureEdit1 = myPictureEdit_Get(FormName, FieldName);

            if ((pictureEdit1 != null) &&
                (((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image != null))
            {
                SaveFileDialog fdialog = new SaveFileDialog();
                fdialog.Filter = "Pictures|*.jpg";

                fdialog.ShowDialog();
                if (fdialog.FileName != "")
                {
                    ((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image.Save(fdialog.FileName);
                }
            }   
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            string FieldName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;
            string FormName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDescription;

            Form tForm = Application.OpenForms[FormName];

            Control pictureEdit1 = myPictureEdit_Get(FormName, FieldName);

            if (pictureEdit1 != null)
            {
                string TableIPCode = ((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Properties.AccessibleName;

                if (t.IsNotNull(TableIPCode))
                {
                    DataSet dsData = null;
                    DataNavigator tDataNavigator = null;
                    t.Find_DataSet(tForm, ref dsData, ref tDataNavigator, TableIPCode);

                    if (dsData != null)
                    {
                        // DB Records
                        tSave sv = new tSave();
                        sv.tDataSave(tForm, dsData, tDataNavigator, tDataNavigator.Position);

                        btn_SaveEnabledChance(tForm, false);
                    }
                }
            }
        }

        private void btn_Orginal_Click(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            string FieldName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleName;
            string FormName = ((DevExpress.XtraEditors.SimpleButton)sender).AccessibleDescription;

            // ----- Silme bu metodda çalışyor
            if (v.con_Images_MasterPath != "")
                Preparing_ImageSave(FormName, FieldName, v.con_Images_MasterPath);
            // -----

            //if (v.con_Image_Original != null)
            //{
            //    Control pictureEdit1 = myPictureEdit_Get(FormName, FieldName);
            //    if (pictureEdit1 != null)
            //    {
            //        // hafızadaki orjinal resmi tekrar pictureEdit yükleniyor
            //        ((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image = v.con_Image_Original.Clone() as Image;
            //    }
            //}
        }
        
        private void webCamCapture1_ImageCaptured(object source, WebCam_Capture.WebcamEventArgs e)
        {
            string FieldName = ((WebCam_Capture.WebCamCapture)source).AccessibleName;
            string FormName = ((WebCam_Capture.WebCamCapture)source).AccessibleDescription;
            
            Control pictureEdit1 = myPictureEdit_Get(FormName, FieldName);
            
            if (pictureEdit1 != null)
            {
                // resmi nesnenin üzerine set ediyor
                ((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image = e.WebCamImage;
            }
        }
                
        private void zoomTrackBarControl1_EditValueChanged(object sender, EventArgs e)
        {
            tToolBox t = new tToolBox();

            string FieldName = ((DevExpress.XtraEditors.ZoomTrackBarControl)sender).AccessibleName;
            string FormName = ((DevExpress.XtraEditors.ZoomTrackBarControl)sender).AccessibleDescription;

            Control pictureEdit1 = myPictureEdit_Get(FormName, FieldName);

            Control label_PictureSize = myPictureSize_Get(FormName, FieldName);

            if (pictureEdit1 != null)
            {
                ((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Properties.ZoomPercent = 
                    ((DevExpress.XtraEditors.ZoomTrackBarControl)sender).Value;

                if (label_PictureSize != null)
                {
                    ((DevExpress.XtraEditors.LabelControl)label_PictureSize).Text = "Zoom Value : " + ((DevExpress.XtraEditors.ZoomTrackBarControl)sender).Value.ToString();
                }
            }
        }

        private Control myPictureEdit_Get(string FormName, string FieldName)
        {
            tToolBox t = new tToolBox();

            Form tForm = Application.OpenForms[FormName];

            string[] controls = new string[] { };

            Control pictureEdit1 = t.Find_Control(tForm, FieldName, "FIELDNAME", controls);

            return pictureEdit1;
        }

        private Control myPictureSize_Get(string FormName, string FieldName)
        {
            //labelControl_PictureSize
            tToolBox t = new tToolBox();

            Form tForm = Application.OpenForms[FormName];

            string[] controls = new string[] { };
            
            Control label_PictureSize = null;

            Control popupContainerEdit1 = t.Find_Control(tForm, "popupContainerEdit1", "", controls);

            if (popupContainerEdit1 != null)
            {
                label_PictureSize = t.Find_PopupContainerEdit(((PopupContainerEdit)popupContainerEdit1), "labelControl_PictureSize");
            }

            return label_PictureSize;
        }

        private void btn_PixelsEnabledChance(Form tForm, bool yeniDurum)
        {
            tToolBox t = new tToolBox();

            string[] controls = new string[] { };

            Control popupContainerEdit1 = t.Find_Control(tForm, "popupContainerEdit1", "", controls);

            if (popupContainerEdit1 != null)
            {
                Control btn_Quality = t.Find_PopupContainerEdit(((PopupContainerEdit)popupContainerEdit1), "btn_Quality");
                Control btn_Compress = t.Find_PopupContainerEdit(((PopupContainerEdit)popupContainerEdit1), "btn_Compress");
                Control btn_AutoCorps = t.Find_PopupContainerEdit(((PopupContainerEdit)popupContainerEdit1), "btn_AutoCorps");

                if (btn_Quality != null) ((DevExpress.XtraEditors.ButtonEdit)btn_Quality).Enabled = yeniDurum;
                if (btn_Compress != null) ((DevExpress.XtraEditors.ButtonEdit)btn_Compress).Enabled = yeniDurum;
                if (btn_AutoCorps != null) ((DevExpress.XtraEditors.ButtonEdit)btn_AutoCorps).Enabled = yeniDurum;
            }
        }

        private void btn_SaveEnabledChance(Form tForm, bool yeniDurum)
        {
            tToolBox t = new tToolBox();

            string[] controls = new string[] { };

            Control popupContainerEdit1 = t.Find_Control(tForm, "popupContainerEdit1", "", controls);

            if (popupContainerEdit1 != null)
            {
                Control btn_Save = t.Find_PopupContainerEdit(((PopupContainerEdit)popupContainerEdit1), "btn_Save");

                if (btn_Save != null)
                    ((DevExpress.XtraEditors.SimpleButton)btn_Save).Enabled = yeniDurum;
            }
        }

        private void Preparing_ImageSave(string FormName, string FieldName, string ImagesPath)
        {
            tToolBox t = new tToolBox();

            if ((t.IsNotNull(FormName) == false) ||
                (t.IsNotNull(FieldName) == false) ||
                (t.IsNotNull(ImagesPath) == false)) return;

            Form tForm = Application.OpenForms[FormName];

            string[] controls = new string[] { };

            Control pictureEdit1 = myPictureEdit_Get(FormName, FieldName);

            Control label_PictureSize = myPictureSize_Get(FormName, FieldName);

            if (pictureEdit1 != null)
            {
                // pictureEdit create sırasında (min kb)VALIDATION_VALUE1 ve (max kb)VALIDATION_VALUE2
                // ataması yapıldıysa buraya kadar şu şekilde geliyor  ( 5.10 )
                                
                string minmax = string.Empty;
                string _min = string.Empty;
                string _max = string.Empty;
                int MinKb = 0;
                int MaxKb = 0;
                double ResimKb = 0;

                // 
                if (((DevExpress.XtraEditors.PictureEdit)pictureEdit1).AccessibleDefaultActionDescription != null)
                {
                    // okunan >>  5.10
                    minmax = ((DevExpress.XtraEditors.PictureEdit)pictureEdit1).AccessibleDefaultActionDescription.ToString();
                    t.TableIPCode_Get(minmax, ref _min, ref _max);
                    // _min >> 5
                    MinKb = t.myInt32(_min);
                    // _max >> 10
                    MaxKb = t.myInt32(_max);
                    // şeklinde resim kb büyüklükleri elde edilir
                    // bunlara bakılarak kaydet ve kb düşürme özelliklerinin kontrolü sağlanır
                }

                Image newImage = Image.FromFile(ImagesPath);
                // resmin orjinal Width ve Heighti   
                // newImage.Width
                // newImage.Height
                
                ((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image = null;
                ((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image = newImage;

                // yeni resmi kaydetmek için binary çevir ve ınsert veya update sırasında kaydet
                long imageLength = 0;
                v.con_Images = null;
                v.con_Images = t.imageBinaryArrayConverter(ImagesPath, ref imageLength);
                v.con_Images_FieldName = FieldName;

                // Bu MasterPath/Image_Original yedek için tutuluyor, 
                // kb. düşürme işlemi sırasında orjinal hali (ilk hali gerekince) kullanılıyor 
                
                // ----- 
                if (v.con_Images_MasterPath == "")
                {
                    v.con_Images_MasterPath = ImagesPath;
                }
                // ----- bu mantık ta çalışyır silme
                //if (v.con_Image_Original == null)
                //{
                //    v.con_Image_Original = newImage.Clone() as Image;
                //}

                

                if (label_PictureSize != null)
                {
                    ResimKb = (float)imageLength / 1024;

                    ((DevExpress.XtraEditors.LabelControl)label_PictureSize).Text =
                        String.Format(
                        "Resim Boyutu : {0:N} kb. ( {1} byte )" + v.ENTER +
                        "Olması gereken boyutu : Min ( {2} kb ), Max ( {3} kb )"
                        , ResimKb.ToString()
                        , imageLength.ToString() 
                        , MinKb.ToString()
                        , MaxKb.ToString() );  
                }

                // bunu açma, form kapanmaya başlıyor
                //newImage.Dispose();
                // ---------------------------------

                v.con_Images_Selection = new Rectangle();

                if ((ResimKb >= MinKb) && (ResimKb <= MaxKb))
                {
                    btn_SaveEnabledChance(tForm, true);
                    btn_PixelsEnabledChance(tForm, false);
                }
                else
                {
                    btn_SaveEnabledChance(tForm, false);
                    btn_PixelsEnabledChance(tForm, true);
                }
                // ---
            }
        }

        private void btn_AutoCorps_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            tToolBox t = new tToolBox();

            string FieldName = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleName;
            string FormName = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleDescription;
            Form tForm = Application.OpenForms[FormName];

            string txt = ((DevExpress.XtraEditors.ButtonEdit)sender).Text;
            int value = t.myInt32(txt);

            // 10 ile 100 arası olsun ( 100 arttırılabilir )

            if (e.Button.Index == 0) value = value - 10;
            if (e.Button.Index == 1) value = value + 10;
            if (value < 10) value = 10;
            if (value > 100) value = 100;

            ((DevExpress.XtraEditors.ButtonEdit)sender).Text = value.ToString() + "  pixel";

            Control pictureEdit1 = myPictureEdit_Get(FormName, FieldName);
            
            // Otomatik Kirp
            
            if ((pictureEdit1 != null) &&
                (((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image != null))
            {
                if (e.Button.Index == 2)
                {
                    string ImagesPath = myImageAutoCorps(tForm, ((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image, value);

                    Preparing_ImageSave(FormName, FieldName, ImagesPath);
                }
            }
        }

        private void btn_Compress_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            tToolBox t = new tToolBox();

            string FieldName = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleName;
            string FormName = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleDescription;
            string txt = ((DevExpress.XtraEditors.ButtonEdit)sender).Text;
            int value = t.myInt32(txt);

            // 10 ile 100 arası olsun ( 100 arttırılabilir )

            if (e.Button.Index == 0) value = value - 10;
            if (e.Button.Index == 1) value = value + 10;
            if (value < 10) value = 10;
            if (value > 100) value = 100;

            ((DevExpress.XtraEditors.ButtonEdit)sender).Text = value.ToString() + "  pixel";
           
            Control pictureEdit1 = myPictureEdit_Get(FormName, FieldName);

            // Sıkıştır

            if ((pictureEdit1 != null) &&
                (((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image != null))
            {
                if (e.Button.Index == 2)
                {
                    string ImagesPath = myImageCompress(((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image, value);

                    Preparing_ImageSave(FormName, FieldName, ImagesPath);
                }
            }
        }

        private void btn_Quality_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            tToolBox t = new tToolBox();

            string FieldName = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleName;
            string FormName = ((DevExpress.XtraEditors.ButtonEdit)sender).AccessibleDescription;
            string txt = ((DevExpress.XtraEditors.ButtonEdit)sender).Text;
            int value = t.myInt32(txt);

            // 10 ile 100 arası olsun ( 100 maximum )

            if (e.Button.Index == 0) value = value - 10;
            if (e.Button.Index == 1) value = value + 10;
            if (value < 10) value = 10;
            if (value > 100) value = 100;

            ((DevExpress.XtraEditors.ButtonEdit)sender).Text = value.ToString() + "  pixel";
                        
            Control pictureEdit1 = myPictureEdit_Get(FormName, FieldName);

            // kalite düşür

            if ((pictureEdit1 != null) &&
                (((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image != null))
            {
                if (e.Button.Index == 2)
                {
                    string ImagesPath = myImageQuality(((DevExpress.XtraEditors.PictureEdit)pictureEdit1).Image, (long)value);

                    Preparing_ImageSave(FormName, FieldName, ImagesPath);
                }
            }
        }
       
        
        /// <summary>
        /// Verilen yoldaki resmi Byte array e çevirir ve return eder.
        /// </summary>
        /// <param name="CevrilecekResimYolu">Hangi resim dosyası üzerinde işlem yapılacak ise yolu.</param>
        /// <returns>byte[] e çevrilmiş şekilde resmi verir.</returns>
        public byte[] myBinaryArrayConverter(string ImagesPath)
        {
            byte[] byteResim = null;
            /*
            FileInfo fInfo = new FileInfo(ImagesPath);
            long sayac = fInfo.Length;
            FileStream fStream = new FileStream(ImagesPath, FileMode.Open, FileAccess.Read);
            BinaryReader bReader = new BinaryReader(fStream);

            byteResim = bReader.ReadBytes((int)sayac);
            v.con_Images_Length = byteResim.Length;
            
            fStream.Close();
            fStream.Dispose();
            bReader.Close();
            bReader.Dispose();
            */
            return byteResim;
        }

        /// <summary>
        /// Verilen resmin kapasitesinin belirtilen parametrede düşürülmesini sağlar.
        /// </summary>
        /// <param name="Resim">İçerisine verilecek resim.</param>
        /// <param name="DosyaIsim">Çıktı olarak verilecek dosyanın adı.</param>
        /// <param name="tip">Kursiyer mi Belge mi seçimi yapılır.</param>
        /// <returns>resmin tam yolunu işlem yapılması adına döndürür.</returns>
        public string myImageQuality(Image Resim, long kalite)
        {
            tToolBox t = new tToolBox();

            //Resim kalitesi düşürülür.
            Bitmap myBitmap;
            ImageCodecInfo myImageCodecInfo;
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter = null;
            EncoderParameters myEncoderParameters;

            myBitmap = new Bitmap(Resim);

            myImageCodecInfo = TipBilgisi("image/jpeg");

            myEncoder = System.Drawing.Imaging.Encoder.Quality;

            myEncoderParameters = new EncoderParameters(1);
            
            // orjinali
            //myEncoderParameter = new EncoderParameter(myEncoder, 65L);

            myEncoderParameter = new EncoderParameter(myEncoder, kalite);

            myEncoderParameters.Param[0] = myEncoderParameter;

            //Klasör oluşturulur ise buraya gelecek, köasür oluşturulup metoda gelen ismiyle dosya oluşturulacak.

            string Images_Path = t.Find_Path("images") + myFileGuidName + ".jpg";

            myBitmap.Save(Images_Path, myImageCodecInfo, myEncoderParameters);
            
            myBitmap.Dispose();
            Resim.Dispose();
            myEncoderParameter.Dispose();
            myEncoderParameters.Dispose();

            return Images_Path;
        }

        /// <summary>
        /// Dışa kapalı metod.Üst bölümde verilen resmin işlenmesine yardımcı olur.
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private ImageCodecInfo TipBilgisi(string mimeType)
        {
            //Üst metodun yardımcı metodu.
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        /// <summary>
        /// Resmin üzerinde verilen h-w ye göre küçültme işlemi yapar.
        /// </summary>
        /// <param name="img">Verilen resim.</param>
        /// <param name="boyut">İstenen h-w.</param>
        /// <param name="Tipi">Kursiyer mi belge mi olduğu.</param>
        /// <returns></returns>
        public string myImageCompress(Image Resim, int mypixel)
        {
            Size yeni_boyut = new Size(-1, -1);
            int kaynakEn = Resim.Width;
            int KaynakBoy = Resim.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            if ((kaynakEn < 20) || (KaynakBoy < 20))  return string.Empty;

            if (mypixel < 10) mypixel = 10;

            yeni_boyut.Width = (kaynakEn - mypixel);
            yeni_boyut.Height = (KaynakBoy - mypixel);

            nPercentW = ((float)yeni_boyut.Width / (float)kaynakEn);
            nPercentH = ((float)yeni_boyut.Height / (float)KaynakBoy);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
            }
            else
            {
                nPercent = nPercentW;
            }

            int HedefEn = (int)(kaynakEn * nPercent);
            int HedefBoy = (int)(KaynakBoy * nPercent);

            // yeni resim hazırlanıyor
            Bitmap b = new Bitmap(HedefEn, HedefBoy, Resim.PixelFormat);

            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
                        
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.InterpolationMode = InterpolationMode.Default;
            // eskiden yeniye aktarım yapılıyor
            g.DrawImage(Resim, 0, 0, HedefEn, HedefBoy);

            return myImagePacket((Image)b);

        }

        /// <summary>
        /// Resmin üzerinde verilen ölçüde kırpma işlemi yapar.
        /// </returns>
        public string myImageAutoCorps(Form tForm, Image Resim, int mypixel)
        {
            // AutoCroping Image - Otomatik Resim Kırp 

            if (mypixel < 10)
                mypixel = 10;

            Bitmap b = null;

            if ((v.con_Images_Selection.Width > 0) &&
                (v.con_Images_Selection.Height > 0))
            {
                // mouse ile seçilen alan
                b = new Bitmap(v.con_Images_Selection.Width, v.con_Images_Selection.Height, Resim.PixelFormat);
            }
            else
            {
                // Auto kırpma
                b = new Bitmap(Resim.Width - (mypixel * 2), Resim.Height - (mypixel * 2), Resim.PixelFormat);
            }
            
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);


            int width = 0;
            int height = 0;
            int x = 0;
            int y = 0;


            // mosue ile seçilmişse
            if ((v.con_Images_Selection.Width > 0) &&
                (v.con_Images_Selection.Height > 0))
            {
                width = v.con_Images_Selection.Width;
                height = v.con_Images_Selection.Height;
                x = v.con_Images_Selection.X;
                y = v.con_Images_Selection.Y;

                // resmi kırpma işlemi
                //g.DrawImage(Resim, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);
                
                // DİKKAT : Henüz koordinatları tam ayarlayamadım.
                g.CopyFromScreen(tForm.Location.X, tForm.Location.Y, 0, 0, new Size(v.con_Images_Selection.Width, v.con_Images_Selection.Height));
                // ---
                v.con_Images_Selection = new Rectangle();
            }
            else
            {
                // Auto kırma 
                width = Resim.Width - mypixel;
                height = Resim.Height - mypixel;
                x = mypixel;
                y = mypixel;

                // resmi kırpma işlemi
                g.DrawImage(Resim, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);

            }
          
            
            return myImagePacket((Image)b);
        }

        /// <summary>
        /// Yeni Resmi yeni bir dosyaya kaydet ve dosya ismini gönder
        /// </returns>
        private string myImagePacket(Image newImage)
        {
            // işlem yapılan yeni resmi alır ve onun için
            // bir dosya hazırlar ve bu dosyanın içine kaydeder.
            // En sonunda da bu dosya ismin geriye dönderir.

            tToolBox t = new tToolBox();

            Bitmap myBitmap = new Bitmap(newImage);

            string Images_Path = t.Find_Path("images") + myFileGuidName + ".jpg";

            myBitmap.Save(Images_Path, ImageFormat.Jpeg);

            myBitmap.Dispose();

            return Images_Path;
        }

        /// <summary>
        /// İstenen sayıda guid döndürür.Sayı çok önemli değil, resim create yapılmasının ardından save işlemi sonrası zaten pics altından resimler temizleniyor.
        /// </summary>
        public string myFileGuidName
        {
            get
            {
                string guid = Guid.NewGuid().ToString();
                return guid.Substring(0, 20);
            }
        }

        //---------------------------------------------------------------------
        private void pictureEdit1_MouseDown(object sender, MouseEventArgs e)
        {
            //// Starting point of the selection:
            //if ((e.Button == MouseButtons.Left) &&
            //    (v.con_Images_Selection.Width <= 0))
            //{
            //    v.con_Images_Selecting = true;
            //    v.con_Images_Selection = new Rectangle(new Point(e.X, e.Y), new Size());
            //}
        }
        
        private void pictureEdit1_MouseMove(object sender, MouseEventArgs e)
        {
            //// Update the actual size of the selection:
            //if (v.con_Images_Selecting)
            //{
            //    v.con_Images_Selection.Width = e.X - v.con_Images_Selection.X;
            //    v.con_Images_Selection.Height = e.Y - v.con_Images_Selection.Y;

            //    // Redraw the picturebox:
            //    ((DevExpress.XtraEditors.PictureEdit)sender).Refresh();
            //}
        }
        
        private void pictureEdit1_MouseUp(object sender, MouseEventArgs e)
        {
            
            //if (e.Button == MouseButtons.Left && v.con_Images_Selecting)
            //{
            //    v.con_Images_Selecting = false;
                                
            //   /*
            //    MessageBox.Show(v.con_Images_Selection.Width.ToString() + " ; " +
            //        v.con_Images_Selection.Height.ToString() + " ; " +
            //        v.con_Images_Selection.X.ToString() + " ; " +
            //        v.con_Images_Selection.Y.ToString());
            //    */
            //    /* 
            // * // Create cropped image:
            //    Image img = ((DevExpress.XtraEditors.PictureEdit)sender).Image.Crop(v.con_Images_Selection);
            //    /*
            //    // Fit image to the picturebox:
            //    ((DevExpress.XtraEditors.PictureEdit)sender).Image = img.Fit2PictureBox(((DevExpress.XtraEditors.PictureEdit)sender));
            
            //    v.con_Images_Selecting = false;
            // **/ 
            //}

            
            
        }
        
        private void pictureEdit1_Paint(object sender, PaintEventArgs e)
        {
            //// Draw a rectangle displaying the current selection
            //if ((v.con_Images_Selection.Height > 0) || (v.con_Images_Selection.Width > 0))
            //{
            //    Pen pen = Pens.GreenYellow;
            //    e.Graphics.DrawRectangle(pen, v.con_Images_Selection);
            //}
        }
        //---------------------------------------------------------------------

        #endregion tUserPictureControl

    }
}
