using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using BottomBorder = DocumentFormat.OpenXml.Spreadsheet.BottomBorder;
using Fill = DocumentFormat.OpenXml.Spreadsheet.Fill;
using Fonts = DocumentFormat.OpenXml.Spreadsheet.Fonts;
using FontScheme = DocumentFormat.OpenXml.Spreadsheet.FontScheme;
using ForegroundColor = DocumentFormat.OpenXml.Spreadsheet.ForegroundColor;
using GradientFill = DocumentFormat.OpenXml.Spreadsheet.GradientFill;
using GradientStop = DocumentFormat.OpenXml.Spreadsheet.GradientStop;
using LeftBorder = DocumentFormat.OpenXml.Spreadsheet.LeftBorder;
using PatternFill = DocumentFormat.OpenXml.Spreadsheet.PatternFill;
using RightBorder = DocumentFormat.OpenXml.Spreadsheet.RightBorder;
using Text = DocumentFormat.OpenXml.Spreadsheet.Text;
using TopBorder = DocumentFormat.OpenXml.Spreadsheet.TopBorder;

namespace MPS.Common
{
    public class MSExcelDocuments
    {
        private static SpreadsheetDocument createParts(string filePath)
        {
            SpreadsheetDocument doc = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);

            WorkbookPart workbookpart = doc.AddWorkbookPart();

            workbookpart.Workbook = new Workbook();

            return doc;
        }

        private static WorksheetPart createWorkSheet(WorkbookPart bookPart, string sheetName = "")
        {
            var sheetPart = bookPart.AddNewPart<WorksheetPart>();

            sheetPart.Worksheet = new Worksheet(new SheetData());

            sheetPart.Worksheet.Save();

            Sheets sheets = bookPart.Workbook.GetFirstChild<Sheets>() ?? bookPart.Workbook.AppendChild(new Sheets());

            string relationshipID = bookPart.GetIdOfPart(sheetPart);

            uint sheetid = 1u;

            if (sheets.Elements<Sheet>().Any())
            {
                sheetid = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            if (string.IsNullOrEmpty(sheetName))
            {
                sheetName = string.Format("Sheet{0}", sheetid);
            }

            var sheet = new Sheet
            {
                Id = relationshipID,
                SheetId = sheetid,
                Name = sheetName
            };

            sheets.Append(new OpenXmlElement[] { sheet });

            bookPart.Workbook.Save();

            return sheetPart;
        }


        private static void createStyleFont(Stylesheet styles)
        {
            styles.Fonts = new Fonts();
            //default id=0
            styles.Fonts.Append(new OpenXmlElement[]
            {
                new Font(new FontSize {Val = 12d},
                    new FontName {Val = "Verdana"},
                    new FontFamily {Val = 2},
                    new FontScheme {Val = FontSchemeValues.Major})
            });
            //header id=1
            styles.Fonts.Append(new OpenXmlElement[]
            {
                new Font(new FontSize {Val = 13d},
                    new FontName {Val = "Verdana"},
                    new FontFamily {Val = 2},
                    new FontScheme {Val = FontSchemeValues.None})
            });

            //cell id=2
            styles.Fonts.Append(new OpenXmlElement[]
            {
                new Font(new FontSize {Val = 12d},
                    new FontName {Val = "Verdana"},
                    new FontFamily {Val = 2},
                    new FontScheme {Val = FontSchemeValues.None})
            });
        }

        private static void createStyleFill(Stylesheet styles)
        {
            styles.Fills = new Fills();
            //fill id=0
            styles.Fills.Append(new OpenXmlElement[] { new Fill(new PatternFill { PatternType = PatternValues.None }) });

            //fill id=1
            styles.Fills.Append(new OpenXmlElement[] { new Fill(new PatternFill { PatternType = PatternValues.Gray125 }) });
            //fill yellow id=2
            styles.Fills.Append(new OpenXmlElement[]
            {
                new Fill(new PatternFill(new ForegroundColor {Rgb = new HexBinaryValue {Value = "FFFFFF00"}})
                {
                    PatternType = PatternValues.Solid
                })
            });
            //fill gray id=3
            styles.Fills.Append(new OpenXmlElement[]
            {
                new Fill(new PatternFill(new ForegroundColor {Rgb = new HexBinaryValue {Value = "FFD3D3D3"}})
                {
                    PatternType = PatternValues.Solid
                })
            });

            //fill green id=4
            styles.Fills.Append(new OpenXmlElement[]
            {
                new Fill(new PatternFill(new ForegroundColor {Rgb = new HexBinaryValue {Value = "FF32cd32"}})
                {
                    PatternType = PatternValues.Solid
                })
            });

            //fill red id=5
            styles.Fills.Append(new OpenXmlElement[]
            {
                new Fill(new PatternFill(new ForegroundColor {Rgb = new HexBinaryValue {Value = "FFff4500"}})
                {
                    PatternType = PatternValues.Solid
                })
            });
            //fill white id=6
            styles.Fills.Append(new OpenXmlElement[]
            {
                new Fill(new PatternFill(new ForegroundColor {Rgb = new HexBinaryValue {Value = "FFffffff"}})
                {
                    PatternType = PatternValues.Solid
                })
            });

            //fill header id=7
            styles.Fills.Append(new OpenXmlElement[]
            {
                new Fill(new GradientFill(
                    new GradientStop {Position = 0, Color = new Color {Rgb = new HexBinaryValue {Value = "ffd9d9d9"}}},
                    new GradientStop {Position = 0.5, Color = new Color {Rgb = new HexBinaryValue {Value = "ffffffff"}}},
                    new GradientStop {Position = 1, Color = new Color {Rgb = new HexBinaryValue {Value = "ffd9d9d9"}}})
                {
                    Degree = 90
                })
            });
        }

        private static void createStyleBorder(Stylesheet styles)
        {
            styles.Borders = new Borders();

            //borderID=0
            var borderDefault = new Border(new LeftBorder(), new RightBorder(), new TopBorder(), new BottomBorder(),
                new DiagonalBorder());
            styles.Borders.Append(new OpenXmlElement[] { borderDefault });

            //borderID=1
            var borderContent = new Border(
                new LeftBorder(new Color { Rgb = new HexBinaryValue { Value = "FF8B4513" } })
                {
                    Style = BorderStyleValues.Thin
                },
                new RightBorder(new Color { Rgb = new HexBinaryValue { Value = "FF8B4513" } })
                {
                    Style = BorderStyleValues.Thin
                },
                new TopBorder(new Color { Rgb = new HexBinaryValue { Value = "FF8B4513" } })
                {
                    Style = BorderStyleValues.Thin
                },
                new BottomBorder(new Color { Rgb = new HexBinaryValue { Value = "FF8B4513" } })
                {
                    Style = BorderStyleValues.Thin
                },
                new DiagonalBorder()
                );
            styles.Borders.Append(new OpenXmlElement[] { borderContent });
        }


        private static void createStyleSheet(WorkbookPart bookPart)
        {
            Stylesheet styles = null;

            if (bookPart.WorkbookStylesPart != null)
            {
                styles = bookPart.WorkbookStylesPart.Stylesheet;
            }

            bookPart.AddNewPart<WorkbookStylesPart>("Style");
            bookPart.WorkbookStylesPart.Stylesheet = new Stylesheet();
            styles = bookPart.WorkbookStylesPart.Stylesheet;

            createStyleFont(styles);

            createStyleFill(styles);

            createStyleBorder(styles);


            styles.CellFormats = new CellFormats();
            //default index=0
            styles.CellFormats.Append(new OpenXmlElement[]
            {
                new CellFormat
                {
                    Alignment = new Alignment(),
                    NumberFormatId = 0,
                    FontId = 0,
                    BorderId = 0,
                    FillId = 0,
                    ApplyAlignment = true,
                    ApplyBorder = true
                }
            });

            //header index=1
            styles.CellFormats.Append(new OpenXmlElement[]
            {
                new CellFormat
                {
                    Alignment = new Alignment(),
                    NumberFormatId = 0,
                    FontId = 1,
                    BorderId = 1,
                    FillId = 7,
                    ApplyAlignment = true,
                    ApplyBorder = true,
                    ApplyFill = true,
                }
            });

            //source index=2
            styles.CellFormats.Append(new OpenXmlElement[]
            {
                new CellFormat
                {
                    Alignment =
                        new Alignment
                        {
                            Horizontal = HorizontalAlignmentValues.Center,
                            Vertical = VerticalAlignmentValues.Center
                        },
                    NumberFormatId = 0,
                    FontId = 2,
                    BorderId = 1,
                    FillId = 3,
                    ApplyAlignment = true,
                    ApplyBorder = true,
                    ApplyFill = true,
                }
            });

            //modify index=3
            styles.CellFormats.Append(new OpenXmlElement[]
            {
                new CellFormat
                {
                    Alignment =
                        new Alignment
                        {
                            Horizontal = HorizontalAlignmentValues.Center,
                            Vertical = VerticalAlignmentValues.Center
                        },
                    NumberFormatId = 0,
                    FontId = 2,
                    BorderId = 1,
                    FillId = 2,
                    ApplyAlignment = true,
                    ApplyBorder = true,
                    ApplyFill = true,
                }
            });

            //new index=4
            styles.CellFormats.Append(new OpenXmlElement[]
            {
                new CellFormat
                {
                    Alignment =
                        new Alignment
                        {
                            Horizontal = HorizontalAlignmentValues.Center,
                            Vertical = VerticalAlignmentValues.Center
                        },
                    NumberFormatId = 0,
                    FontId = 2,
                    BorderId = 1,
                    FillId = 4,
                    ApplyAlignment = true,
                    ApplyBorder = true,
                    ApplyFill = true,
                }
            });

            //normal index=5
            styles.CellFormats.Append(new OpenXmlElement[]
            {
                new CellFormat
                {
                    Alignment =
                        new Alignment
                        {
                            Horizontal = HorizontalAlignmentValues.Center,
                            Vertical = VerticalAlignmentValues.Center
                        },
                    NumberFormatId = 0,
                    FontId = 2,
                    BorderId = 1,
                    FillId = 0,
                    ApplyAlignment = true,
                    ApplyBorder = true,
                    ApplyFill = true,
                }
            });

            bookPart.WorkbookStylesPart.Stylesheet.Save();
        }

        private static string GetCellReference(int colIndex)
        {
            int dividend = colIndex;

            string colName = string.Empty;

            while (dividend > 0)
            {
                int modifyer = (dividend - 1) % 26;

                colName = Convert.ToChar(65 + modifyer) + colName;

                dividend = ((dividend - modifyer) / 26);
            }

            return colName;
        }

        private static Cell createTextCell(int columnIndex, int rowIndex, object cellValue, uint? styleIndex)
        {
            var cell = new Cell
            {
                DataType = CellValues.InlineString,
                CellReference = GetCellReference(columnIndex) + rowIndex
            };

            if (styleIndex.HasValue)
            {
                cell.StyleIndex = styleIndex.Value;
            }

            var inlineString = new InlineString();

            var t = new Text { Text = cellValue.ToString() };

            inlineString.AppendChild(t);

            cell.AppendChild(inlineString);

            return cell;
        }

        private static Cell CreateValueCell(int columnIndex, int rowIndex, object cellvalue, uint? styleIndex)
        {
            var cell = new Cell
            {
                CellReference = GetCellReference(columnIndex) + rowIndex,
            };

            var value = new CellValue { Text = cellvalue.ToString() };

            if (styleIndex.HasValue)
            {
                cell.StyleIndex = styleIndex.Value;
            }

            cell.AppendChild(value);

            return cell;
        }

        private static Row CreateDataRow(DataRow dataRow, int rowIndex)
        {
            var row = new Row { RowIndex = (UInt32)rowIndex };

            for (int i = 0; i < dataRow.Table.Columns.Count; i++)
            {
                Cell cell;

                DateTime dateValue;

                decimal decValue;

                double dblValue;

                int intValue;

                if (DateTime.TryParse(dataRow[i].ToString(), out dateValue) && dataRow[i] is DateTime)
                {
                    cell = createTextCell(i + 1, rowIndex, dateValue, 5u);
                }
                else if (decimal.TryParse(dataRow[i].ToString(), out decValue) && dataRow[i] is decimal)
                {
                    cell = CreateValueCell(i + 1, rowIndex, decValue, 5u);
                }
                else if (double.TryParse(dataRow[i].ToString(), out dblValue) && dataRow[i] is double)
                {
                    cell = CreateValueCell(i + 1, rowIndex, dblValue, 5u);
                }
                else if (int.TryParse(dataRow[i].ToString(), out intValue) && dataRow[i] is int)
                {
                    cell = CreateValueCell(i + 1, rowIndex, intValue, 5u);
                }
                else
                {
                    cell = createTextCell(i + 1, rowIndex, dataRow[i], 5u);
                }
                row.AppendChild(cell);
            }
            return row;
        }

        private static void InsertDataIntoSheet(DataTable dt, OpenXmlCompositeElement sheet)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Row contentRow = CreateDataRow(dt.Rows[i], i + 3);

                sheet.AppendChild(contentRow);
            }
        }

        private static Row createContentRow(DataRow dataRow, int rowIndex, string dirPath = "", Hyperlinks links = null, WorksheetPart sheetPart = null)
        {
            var row = new Row

            {
                RowIndex = (UInt32)rowIndex
            };

            for (int i = 0; i < dataRow.Table.Columns.Count; i++)
            {
                var dataCell = createTextCell(i + 1, rowIndex, dataRow[i], 5u);
                //var dataCell = CreateValueCell(i + 1, rowIndex, dataRow[i], 5u);
                if (links != null && sheetPart != null)
                {
                    var value = dataRow[i].ToString();

                    if (File.Exists(value))
                    {
                        var hID = string.Format("h{0}", dataCell.CellReference.Value);

                        links.AppendChild(new Hyperlink { Reference = dataCell.CellReference.Value, Id = hID });

                        sheetPart.AddHyperlinkRelationship(
                            new Uri(value.ToLower().Replace(dirPath.ToLower(), ""), UriKind.RelativeOrAbsolute), true, hID);
                    }
                }
                row.AppendChild(dataCell);
            }
            return row;
        }




        public static void ExportToExcel(string filePath, DataSet ds)
        {
            var dirPath = filePath.Remove(filePath.LastIndexOf('\\') + 1);

            var doc = createParts(filePath);
            createStyleSheet(doc.WorkbookPart);


            foreach (DataTable dt in ds.Tables)
            {
                var sheetPart = createWorkSheet(doc.WorkbookPart, dt.TableName);

                var links = new Hyperlinks();

                var data = sheetPart.Worksheet.GetFirstChild<SheetData>();

                var header = new Row();

                foreach (DataColumn column in dt.Columns)
                {
                    Cell headerCell = createTextCell(dt.Columns.IndexOf(column) + 1, 1, column.ColumnName, 1u);
                    header.AppendChild(headerCell);
                }
                data.AppendChild(header);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];

                    data.AppendChild(createContentRow(row, i + 2, dirPath, links, sheetPart));
                }
                if (links.Any())
                {
                    sheetPart.Worksheet.AppendChild(links);
                }
            }

            doc.Close();
        }

        public static DataSet ReadExcelDocument(string filePath)
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(filePath, false))
            {
                var workBook = doc.WorkbookPart.Workbook;

                var part = doc.WorkbookPart;
                var shared = doc.WorkbookPart.SharedStringTablePart;
                var sharedStrings = shared != null ? shared.SharedStringTable : null;
                var sheets = workBook.Descendants().OfType<Sheet>();

                var ds = new DataSet();

                foreach (var sheet in sheets.Select(s => new KeyValuePair<string, WorksheetPart>(s.Name, (WorksheetPart)part.GetPartById(s.Id))))
                {

                    DataTable dt = null;
                    foreach (var r in sheet.Value.Worksheet.Descendants().OfType<Row>())
                    {
                        var textValues = (from cell in r.Descendants<Cell>()
                                          select
                                                cell.CellValue == null ? cell.InnerText :
                                                (cell.DataType != null
                                                    && cell.DataType.HasValue
                                                    && cell.DataType == CellValues.SharedString && sharedStrings != null)
                                                ? sharedStrings.ChildElements[
                                                    int.Parse(cell.CellValue.InnerText)].InnerText
                                                : cell.CellValue.InnerText).ToArray();


                        if (dt == null)
                        {
                            dt = new DataTable(sheet.Key);

                            for (int i = 0; i < textValues.Length; i++)
                            {
                                dt.Columns.Add(string.Format("Column{0}", i));
                            }
                        }

                        var row = dt.Rows.Add();


                        for (int i = 0; i < textValues.Length; i++)
                        {
                            if (dt.Columns.Count == i) break;

                            row[i] = textValues[i];
                        }

                    }

                    ds.Tables.Add(dt);

                }

                return ds;
            }
        }

        public static DataSet ReadExcelDocument(string filePath, DataColumn[] tableColums)
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(filePath, false))
            {
                var workBook = doc.WorkbookPart.Workbook;

                var part = doc.WorkbookPart;
                var shared = doc.WorkbookPart.SharedStringTablePart;
                var sharedStrings = shared != null ? shared.SharedStringTable : null;
                var sheets = workBook.Descendants().OfType<Sheet>();

                var ds = new DataSet();

                foreach (var sheet in sheets.Select(s => new KeyValuePair<string, WorksheetPart>(s.Name, (WorksheetPart)part.GetPartById(s.Id))))
                {

                    DataTable dt = null;
                    foreach (var r in sheet.Value.Worksheet.Descendants().OfType<Row>())
                    {
                        var textValues = (from cell in r.Descendants<Cell>()
                                          select
                                                cell.CellValue == null ? cell.InnerText :
                                                (cell.DataType != null
                                                    && cell.DataType.HasValue
                                                    && cell.DataType == CellValues.SharedString && sharedStrings != null)
                                                ? sharedStrings.ChildElements[
                                                    int.Parse(cell.CellValue.InnerText)].InnerText
                                                : cell.CellValue.InnerText).ToArray();




                        if (dt == null)
                        {
                            dt = new DataTable(sheet.Key);
                            //dt.Columns.AddRange(tableColums);

                            for (int i = 0; i < textValues.Length; i++)
                            {
                                dt.Columns.Add(tableColums[i].ToString());
                            }
                        }

                        var row = dt.Rows.Add();


                        for (int i = 0; i < textValues.Length; i++)
                        {
                            if (dt.Columns.Count == i) break;

                            row[i] = textValues[i];
                        }

                    }
                    if (dt != null)
                        ds.Tables.Add(dt);
                }

                return ds;
            }
        }



        public static void ExportToExcel(string filePath, DataTable dt)
        {
            var dirPath = filePath.Remove(filePath.LastIndexOf('\\') + 1);

            var doc = createParts(filePath);
            createStyleSheet(doc.WorkbookPart);
            var sheetPart = createWorkSheet(doc.WorkbookPart, dt.TableName);

            var links = new Hyperlinks();



            var data = sheetPart.Worksheet.GetFirstChild<SheetData>();

            var header = new Row();

            foreach (DataColumn column in dt.Columns)
            {
                Cell headerCell = createTextCell(dt.Columns.IndexOf(column) + 1, 1, column.ColumnName, 1u);
                header.AppendChild(headerCell);
            }
            data.AppendChild(header);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];

                data.AppendChild(createContentRow(row, i + 2, dirPath, links, sheetPart));
            }
            if (links.Any())
            {
                sheetPart.Worksheet.AppendChild(links);
            }
            doc.Close();
        }

        private static Columns AutoFit(SheetData sheetData)
        {
            var maxColWidth = GetMaxCharacterWidth(sheetData);

            Columns columns = new Columns();

            double maxWidth = 7;
            foreach (var item in maxColWidth)
            {
                /*三种单位宽度公式*/
                double width = Math.Truncate((item.Value * maxWidth + 5) / maxWidth * 256) / 256;
                double pixels = Math.Truncate(((256 * width + Math.Truncate(128 / maxWidth)) / 256) * maxWidth);
                double charWidth = Math.Truncate((pixels - 5) / maxWidth * 100 + 0.5) / 100;

                Column col = new Column() { BestFit = true, Min = (UInt32)(item.Key + 1), Max = (UInt32)(item.Key + 1), CustomWidth = true, Width = (DoubleValue)width };
                columns.Append(col);
            }
            return columns;
        }

        private static Dictionary<int, int> GetMaxCharacterWidth(SheetData sheetData)
        {
            Dictionary<int, int> maxColWidth = new Dictionary<int, int>();
            var rows = sheetData.Elements<Row>();
            foreach (var r in rows)
            {
                var cells = r.Elements<Cell>().ToArray();
                for (int i = 0; i < cells.Length; i++)
                {
                    var cell = cells[i];
                    var cellValue = cell.CellValue == null ? string.Empty : cell.CellValue.InnerText;
                    var cellTextLength = cellValue.Length;
                    if (maxColWidth.ContainsKey(i))
                    {
                        var current = maxColWidth[i];
                        if (cellTextLength > current)
                        {
                            maxColWidth[i] = cellTextLength;
                        }
                    }
                    else
                    {
                        maxColWidth.Add(i, cellTextLength);
                    }
                }
            }
            return maxColWidth;
        }
    }
}
