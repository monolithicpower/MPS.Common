using MPS.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace MonolithicPowerSystem.AE.Common.Parser
{
    public class SchemaParser
    {
        public static ProjectConfig XmlToModel(string path)
        {
            XNamespace ns = "http://www.monolithicpower.com/XMLSchema";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            var doc = XDocument.Load(path);
            var checksumDb = doc.Root.Attribute("ChipsChecksum").Value;
            var chipsElement = doc.Root.Element(ns + "Chips");
            if (chipsElement == null)
                return null;
            var md5Calc = getMD5String(chipsElement.ToString());
            if (!string.Equals(md5Calc, checksumDb))
            {
                var dResult = MessageBox.Show("Do you still want to Load?", "The file changed", MessageBoxButton.YesNo);
                if (dResult == MessageBoxResult.No)
                    return null;
            }
            ProjectConfig config = new ProjectConfig();
            config.Chips = new System.Collections.ObjectModel.ObservableCollection<Chip>();

            List<Chip> chips = new List<Chip>();
            foreach (var item in doc.Descendants(ns + "Chip"))
            {
                Chip tempChip = new Chip()
                {
                    ChipAddr = item.Attribute("Address").Value,
                    ChipName = item.Attribute("Name").Value,
                };

                List<Page> pages = new List<Page>();

                foreach (var reg in item.Descendants(ns + "Register"))
                {
                    List<string> groupNames = new List<string>();
                    List<string> pageNames = new List<string>();
                    List<GroupItemInfo> regGroups = new List<MPS.Common.Model.GroupItemInfo>();
                    #region page

                    foreach (var pg in reg.Descendants(ns + "Page"))
                    {

                        var tmpPageName = pg.Attribute("Name").Value;
                        if (!pageNames.Contains(tmpPageName))
                            pageNames.Add(pg.Attribute("Name").Value);
                    }
                    #endregion

                    #region Group



                    foreach (var gps in reg.Descendants(ns + "DisplayGroup"))
                    {
                        var tmpGroupName = gps.Attribute("Name").Value;
                        if (!groupNames.Contains(tmpGroupName))
                            groupNames.Add(gps.Attribute("Name").Value);

                    }
                    #endregion


                    //Register tReg = new Register();
                    //tReg.Address = Convert.ToByte(reg.Attribute("Address").Value, 16);
                    //tReg.Name = reg.Attribute("Name").Value;
                    //tReg.DataType = reg.Attribute("DataType").Value;
                    //tReg.Right = reg.Attribute("Right").Value;
                    //tReg.BitControls = new List<MPS.Common.Model.BitControl>();
                    //var tBitControls = new List<MPS.Common.Model.BitControl>();
                    //foreach (var bt in reg.Descendants(ns + "BitControl"))
                    //{
                    //    BitControl btControl = new MPS.Common.Model.BitControl();

                    //    btControl.Unit = bt.Attribute("Unit") == null ? "" : bt.Attribute("Unit").Value;
                    //    btControl.BitName = bt.Attribute("Name").Value;
                    //    btControl.Position = Convert.ToByte(bt.Attribute("Position").Value);
                    //    btControl.BitLength = Convert.ToByte(bt.Attribute("BitLength").Value);
                    //    var at = bt.Attribute(xsi + "type").Value;
                    //    if (at == "PossibleValuesType")
                    //    {
                    //        List<string> pss = new List<string>();
                    //        foreach (var ps in bt.Descendants(ns + "PossibleValue"))
                    //        {
                    //            pss.Add(string.Format("{0}[{1}]", ps.Attribute("Name").Value, ps.Attribute("Value").Value));
                    //        }
                    //        btControl.PossibleValues = string.Join(";", pss);
                    //    }
                    //    else
                    //    {
                    //        btControl.Lsb = Convert.ToDouble(bt.Attribute("Lsb").Value);
                    //        btControl.MinValue = Convert.ToDouble(bt.Attribute("MinValue").Value);
                    //        btControl.MaxValue = Convert.ToDouble(bt.Attribute("MaxValue").Value);
                    //    }
                    //    // tReg.BitControls.Add(btControl);
                    //    tBitControls.Add(btControl);
                    //}


                    foreach (var pageName in pageNames)
                    {
                        Register tReg = new Register();
                        tReg.Address = Convert.ToByte(reg.Attribute("Address").Value, 16);
                        tReg.Name = reg.Attribute("Name").Value;
                        tReg.DataType = reg.Attribute("DataType").Value;
                        tReg.Right = reg.Attribute("Right").Value;
                        tReg.BitControls = new List<MPS.Common.Model.BitControl>();

                        //var tBitControls = new List<MPS.Common.Model.BitControl>();
                        foreach (var bt in reg.Descendants(ns + "BitControl"))
                        {
                            BitControl btControl = new MPS.Common.Model.BitControl();

                            btControl.Unit = bt.Attribute("Unit") == null ? "" : bt.Attribute("Unit").Value;
                            btControl.BitName = bt.Attribute("Name").Value;
                            btControl.Position = Convert.ToByte(bt.Attribute("Position").Value);
                            btControl.BitLength = Convert.ToByte(bt.Attribute("BitLength").Value);
                            var at = bt.Attribute(xsi + "type").Value;
                            if (at == "PossibleValuesType")
                            {
                                List<string> pss = new List<string>();
                                foreach (var ps in bt.Descendants(ns + "PossibleValue"))
                                {
                                    pss.Add(string.Format("{0}[{1}]", ps.Attribute("Name").Value, ps.Attribute("Value").Value));
                                }
                                btControl.PossibleValues = string.Join(";", pss);
                            }
                            else
                            {
                                btControl.Lsb = Convert.ToDouble(bt.Attribute("Lsb").Value);
                                btControl.MinValue = Convert.ToDouble(bt.Attribute("MinValue").Value);
                                btControl.MaxValue = Convert.ToDouble(bt.Attribute("MaxValue").Value);
                            }
                            tReg.BitControls.Add(btControl);
                            //tBitControls.Add(btControl);
                        }



                        if (!pages.Any(o => o.PageName == pageName))
                        {
                            pages.Add(new MPS.Common.Model.Page() { PageName = pageName, GroupItems = new System.Collections.ObjectModel.ObservableCollection<MPS.Common.Model.GroupItemInfo>() });

                        }
                        foreach (var groupName in groupNames)
                        {
                            if (pages.Where(o => o.PageName == pageName).FirstOrDefault().GroupItems.Any(o => o.GroupItemName == groupName))
                            {
                                //pages.Where(o => o.PageName == pageName).FirstOrDefault().GroupItems.Where(o => o.GroupItemName == groupName).FirstOrDefault().Registers.Add(tReg);
                            }
                            else
                            {
                                GroupItemInfo tmp = new MPS.Common.Model.GroupItemInfo()
                                {
                                    GroupItemName = groupName,
                                    Registers = new System.Collections.ObjectModel.ObservableCollection<MPS.Common.Model.Register>()
                                };
                                pages.Where(o => o.PageName == pageName).FirstOrDefault().GroupItems.Add(tmp);
                            }
                            var cReg = doc.Descendants(ns + "Register").Where(o => o.Attribute("Name").Value == tReg.Name).FirstOrDefault();
                            tReg.Value = Convert.ToInt32(cReg.Descendants(ns + "Page").Where(o => o.Attribute("Name").Value == pageName).FirstOrDefault().Attribute("RegisterValue").Value, 16);
                            ExplainBitValue(tReg);
                            pages.Where(o => o.PageName == pageName).FirstOrDefault().GroupItems.Where(o => o.GroupItemName == groupName).FirstOrDefault().Registers.Add(tReg);

                        }

                    }

                }
                tempChip.Pages = new System.Collections.ObjectModel.ObservableCollection<Page>(pages);
                config.Chips.Add(tempChip);
            }
            //MPS.Common.Serializer.SerializeHelper.XMLSerialize(config, @"E:\New Schema test\8855TEST.xml");
            //ModelToXml(config, null);
            return config;

        }

        public static void ModelToXml(ProjectConfig config, string path)
        {
            XNamespace ns = "http://www.monolithicpower.com/XMLSchema";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            XDocument xDoc = new XDocument();
            XElement root = new XElement(ns + "ProjectConfig", new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                new XAttribute("LastModifiedTool", System.Diagnostics.Process.GetCurrentProcess().ProcessName.Split(new char[] { '.' })[0]),
                 new XAttribute("CreatedTool", System.Diagnostics.Process.GetCurrentProcess().ProcessName.Split(new char[] { '.' })[0]),
                new XAttribute("CreatedDate", DateTime.Now),
                new XAttribute("LastModifiedDate", DateTime.Now),
                new XAttribute("CreatedToolVersion", version),
                new XAttribute("LastModifiedToolVersion", version),

                new XAttribute("ChipsChecksum", ""));
            // XElement root = new XElement("ProjectConfig");

            xDoc.Add(root);
            XElement tChips = new XElement(ns + "Chips");
            root.Add(tChips);
            //xDoc.Save(@"E:\TOXML.xml");

            foreach (var chip in config.Chips)
            {
                XElement tChip = new XElement(ns + "Chip", new XAttribute("Name", chip.ChipName), new XAttribute("Address", chip.ChipAddr));
                tChips.Add(tChip);
                XElement tRegs = new XElement(ns + "Registers");
                tChip.Add(tRegs);
                foreach (var page in chip.Pages)
                {
                    foreach (var group in page.GroupItems)
                    {
                        foreach (var reg in group.Registers)
                        {
                            ParseRegisterValue(reg);

                            XElement bReg = tRegs.Elements(ns + "Register").Where(o => o.Attribute("Name").Value == reg.Name).FirstOrDefault();
                            if (bReg != null)
                            {
                                // var xe = bReg.Descendants(ns+ "DisplayGroup").Where(o => o.Attribute("Name").Value == group.GroupItemName);
                                if (!bReg.Descendants(ns + "DisplayGroup").Any(o => o.Attribute("Name").Value == group.GroupItemName))
                                {
                                    XElement moreGroupitem = new XElement(ns + "DisplayGroup", new XAttribute("Name", group.GroupItemName));
                                    XElement moreDisplays = bReg.Element(ns + "DisplayGroups");
                                    moreDisplays.Add(moreGroupitem);
                                }

                                if (bReg.Descendants(ns + "Page").Count() > 0)
                                {
                                    if (bReg.Descendants(ns + "Page").Any(o => o.Attribute("Name").Value == page.PageName))
                                        continue;
                                }

                                XElement morePage = new XElement(ns + "Page", new XAttribute("Name", page.PageName),
                               new XAttribute("RegisterValue", string.Format("0x{0}", Convert.ToString(reg.Value, 16))));
                                XElement morePages = bReg.Element(ns + "Pages");
                                morePages.Add(morePage);
                                continue;
                            }

                            //cp.ChipAddr = $"0x{addAddr / 16:x}{addAddr % 16:x}";
                            XElement tReg = new XElement(ns + "Register", new XAttribute("Name", reg.Name), new XAttribute("Address", $"0x{reg.Address / 16:x}{reg.Address % 16:x}"),
                            new XAttribute("DataType", reg.DataType), new XAttribute("Right", reg.Right));
                            tRegs.Add(tReg);
                            XElement tBits = new XElement(ns + "BitControls");
                            tReg.Add(tBits);
                            foreach (var bit in reg.BitControls)
                            {
                                XElement tBit;
                                if (string.IsNullOrEmpty(bit.PossibleValues))
                                {
                                    tBit = new XElement(ns + "BitControl", new XAttribute(xsi + "type", "MinMaxType"),
                                        new XAttribute("Name", bit.BitName), new XAttribute("Position", bit.Position),
                                        new XAttribute("BitLength", bit.BitLength), new XAttribute("Lsb", bit.Lsb),
                                        new XAttribute("MinValue", bit.MinValue), new XAttribute("MaxValue", bit.MaxValue),
                                        new XAttribute("Unit", bit.Unit ?? ""));

                                }
                                else
                                {
                                    tBit = new XElement(ns + "BitControl", new XAttribute(xsi + "type", "PossibleValuesType"),
                                        new XAttribute("Name", bit.BitName), new XAttribute("Position", bit.Position),
                                        new XAttribute("BitLength", bit.BitLength));

                                    XElement tPossibleValues = new XElement(ns + "PossibleValues");
                                    tBit.Add(tPossibleValues);
                                    var possibleValues = bit.PossibleValues.Split(new char[] { ';' });
                                    foreach (var possibleValue in possibleValues)
                                    {
                                        var keyvalue = possibleValue.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                                        if (keyvalue.Length < 2)
                                            continue;
                                        XElement tPossibleValue = new XElement(ns + "PossibleValue", new XAttribute("Name", keyvalue[0]),
                                            new XAttribute("Value", keyvalue[1]));
                                        tPossibleValues.Add(tPossibleValue);

                                    }

                                }
                                tBits.Add(tBit);

                            }

                            XElement tPages = new XElement(ns + "Pages");
                            tReg.Add(tPages);

                            XElement tPage = new XElement(ns + "Page", new XAttribute("Name", page.PageName),
                                new XAttribute("RegisterValue", string.Format("0x{0}", Convert.ToString(reg.Value, 16))));
                            tPages.Add(tPage);

                            XElement tDisplayGroups = new XElement(ns + "DisplayGroups");
                            tReg.Add(tDisplayGroups);
                            XElement tDisplayGroup = new XElement(ns + "DisplayGroup", new XAttribute("Name", group.GroupItemName));
                            tDisplayGroups.Add(tDisplayGroup);

                        }
                    }

                }
            }
            var checkSumText = xDoc.Root.Element(ns + "Chips").ToString();
            var checkSum = getMD5String(checkSumText);
            xDoc.Root.SetAttributeValue("ChipsChecksum", checkSum);
            xDoc.Save(path);

        }

        private static void ParseRegisterValue(Register reg)
        {
            foreach (var bt in reg.BitControls)
            {
                if (string.IsNullOrEmpty(bt.PossibleValues))
                {
                    if (bt.CurrentBitValue == null)
                        continue;
                    var value = bt.CurrentBitValue;
                    var dac = (int)(double.Parse(value) / bt.Lsb + 0.5);
                    bt.CurrentBitValue = (dac * bt.Lsb).ToString();
                    var bitBinary = Convert.ToString(dac, 2);
                    if (bitBinary.Length < bt.BitLength)
                    {
                        bitBinary = bitBinary.PadLeft(bt.BitLength, '0');
                    }
                    else if (bitBinary.Length > bt.BitLength)
                    {
                        bitBinary = bitBinary.Remove(0, bitBinary.Length - bt.BitLength);
                    }
                    var temp = Convert.ToString(reg.Value, 2);
                    if (reg.DataType == EnumCommandType.Word.ToString())
                    {
                        if (temp.Length < 16)
                        {
                            temp = temp.PadLeft(16, '0');
                        }
                        var selectedValue = Convert.ToUInt16((temp.Remove(16 - bt.Position - bt.BitLength, bt.BitLength).Insert(16 - bt.Position - bt.BitLength, bitBinary)), 2);
                        reg.Value = selectedValue;
                    }
                    else
                    {
                        if (temp.Length < 8)
                        {
                            temp = temp.PadLeft(8, '0');
                        }
                        var selectedValue = Convert.ToByte((temp.Remove(8 - bt.Position - bt.BitLength, bt.BitLength).Insert(8 - bt.Position - bt.BitLength, bitBinary)), 2);
                        reg.Value = selectedValue;
                    }
                }
                else
                {
                    if (bt.CurrentBitValue == null)
                        continue;
                    var byteValue = bt.CurrentBitValue.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                    if (byteValue.Length < 2)
                        continue;
                    int selectedValue = reg.Value;
                    if (byteValue[1].StartsWith("0x"))
                    {
                        selectedValue = Convert.ToUInt16(byteValue[1], 16);
                    }
                    else if (byteValue[1].StartsWith("0b"))
                    {
                        var temp = Convert.ToString(reg.Value, 2);
                        if (reg.DataType == EnumCommandType.Word.ToString())
                        {
                            if (temp.Length < 16)
                            {
                                temp = temp.PadLeft(16, '0');
                            }
                            selectedValue = Convert.ToUInt16((temp.Remove(16 - bt.Position - bt.BitLength, bt.BitLength).Insert(16 - bt.Position - bt.BitLength, byteValue[1].Remove(0, 2))), 2);
                        }
                        else
                        {
                            if (temp.Length < 8)
                            {
                                temp = temp.PadLeft(8, '0');
                            }
                            selectedValue = Convert.ToByte((temp.Remove(8 - bt.Position - bt.BitLength, bt.BitLength).Insert(8 - bt.Position - bt.BitLength, byteValue[1].Remove(0, 2))), 2);
                        }
                    }
                    reg.Value = selectedValue;

                }

            }

        }

        private static void ExplainBitValue(Register reg)
        {

            foreach (var bitcontrol in reg.BitControls)
            {
                if (string.IsNullOrEmpty(bitcontrol.PossibleValues))
                {
                    StringBuilder strb = new StringBuilder();
                    for (int i = bitcontrol.Position + bitcontrol.BitLength - 1; i >= bitcontrol.Position; i--)
                    {
                        strb.Append((reg.Value >> i) & 1);
                    }
                    var bitDac = Convert.ToInt16(strb.ToString(), 2);

                    bitcontrol.CurrentBitValue = bitcontrol.SourceBitValue = Convert.ToString(bitDac * bitcontrol.Lsb);
                }
                else
                {
                    StringBuilder strb = new StringBuilder();
                    for (int i = bitcontrol.Position + bitcontrol.BitLength - 1; i >= bitcontrol.Position; i--)
                    {
                        strb.Append((reg.Value >> i) & 1);
                    }
                    var pvs = bitcontrol.PossibleValues.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim());

                    foreach (var pv in pvs)
                    {
                        var temps = pv.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                        if (temps.Length < 2)
                            continue;
                        if (temps.Any(o => o == string.Format("0b{0}", strb.ToString())))
                        {
                            bitcontrol.CurrentBitValue = bitcontrol.SourceBitValue = pv;
                            break;
                        }
                        else if (temps[1].StartsWith("0x"))
                        {
                            if (Convert.ToInt16(temps[1], 16) == reg.Value)
                            {
                                bitcontrol.CurrentBitValue = bitcontrol.SourceBitValue = pv;
                                break;
                            }

                        }
                    }

                }

            }
        }

        private static string getMD5String(string original)
        {
            string hash;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                hash = BitConverter.ToString(
                  md5.ComputeHash(Encoding.UTF8.GetBytes(original))
                ).Replace("-", String.Empty);
            }
            return hash;
        }

    }
}

