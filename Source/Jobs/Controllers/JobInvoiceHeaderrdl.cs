﻿using Data.Models;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using Model.Models;
using System.IO;
using System.Collections.Generic;
using ImageResizer;
using System;
using Core.Common;

namespace Jobs.Controllers
{
    public class JobInvoiceHeaderRDL
    {

        public string DocPrint_JobInvoice()
        {
            string StringCode = "";
            StringCode = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Report xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition"" xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"">
  <Body>
    <ReportItems>
      <Tablix Name=""Tablix1"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>1.04563in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.1616in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>3.40766in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.22528in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox27"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox26</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""BuyerName"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Format>0.0000;(0.0000);'-'</Format>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>BuyerName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Jobworker"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Jobworker</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox3</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox117"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox117</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox134"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartySuffix.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox134</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.2357in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox196"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Address</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox177</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox76"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox60</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Address"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyAddress.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Address</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.22528in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox252"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Mobile No.</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox252</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox81"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox60</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""MobileNo"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PartyMobileNo.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>MobileNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.23958in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox149"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>GSTIN</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox146</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox143"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox143</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox150"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Party_GST_NO.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox145</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.23362in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox115"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value EvaluationMode=""Constant"">Aadhar No</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox115</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox80"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox60</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""TinNo"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Party_AADHAR_NO.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>TinNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.21875in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox147"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value EvaluationMode=""Constant"">PAN No</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox147</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox148"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox148</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""AADHAR"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!Party_PAN_NO.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>AADHAR</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember>
              <Visibility>
                <Hidden>=iif(fields!PartySuffix.Value &lt;&gt; """",false,true)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember />
            <TablixMember>
              <Visibility>
                <Hidden>=iif(fields!PartyMobileNo.Value &lt;&gt; """",false,true)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(max(fields!Party_GST_NO.Value) &lt;&gt; """",False,True)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(max(fields!Party_AADHAR_NO.Value) &lt;&gt; """",False,True)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(fields!Party_PAN_NO.Value &lt;&gt; """" ,false,true)</Hidden>
              </Visibility>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>0.66598in</Top>
        <Left>0.03454in</Left>
        <Height>1.62821in</Height>
        <Width>4.61489in</Width>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix5"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>1.19473in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.11461in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.79351in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.19132in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox39"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!DocIdCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox34</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox52"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox52</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocNo"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocNo.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.21216in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox36"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!DocIdCaptionDate.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox31</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox49"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox49</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DocDate"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DocDate.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <Format>dd/MMM/yy</Format>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DocDate</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox6"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!PartyDocCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox6</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox7"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox7</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox9"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!PartyDocNo.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox9</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox10"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!PartyDocDateCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox10</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox11"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox11</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox14"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!PartyDocDate.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox14</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.19792in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox194"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Reverse Charge</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox194</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox195"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>:</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                                <Color>#4d4d4d</Color>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox195</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                          <Style>None</Style>
                          <Width>1pt</Width>
                        </TopBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ReverseCharge"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!ReverseCharge.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ReverseCharge</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember>
              <Visibility>
                <Hidden>=iif(fields!PartyDocNo.Value &lt;&gt; """",false,true)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(Fields!PartyDocDate.Value &lt;&gt; """",false,true)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember>
              <Visibility>
                <Hidden>=iif(max(fields!ReverseCharge.Value) &lt;&gt; """",False,True)</Hidden>
              </Visibility>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>0.67223in</Top>
        <Left>4.68335in</Left>
        <Height>1.1014in</Height>
        <Width>3.10285in</Width>
        <ZIndex>1</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Textbox Name=""ReportTitle"">
        <CanGrow>true</CanGrow>
        <KeepTogether>true</KeepTogether>
        <Paragraphs>
          <Paragraph>
            <TextRuns>
              <TextRun>
                <Value>=First(Fields!ReportTitle.Value, ""DsMain"")</Value>
                <Style>
                  <FontFamily>Tahoma</FontFamily>
                  <FontSize>14pt</FontSize>
                  <FontWeight>Bold</FontWeight>
                </Style>
              </TextRun>
            </TextRuns>
            <Style>
              <TextAlign>Center</TextAlign>
            </Style>
          </Paragraph>
        </Paragraphs>
        <rd:DefaultName>ReportTitle</rd:DefaultName>
        <Top>0.35734in</Top>
        <Left>0.01872in</Left>
        <Height>0.27183in</Height>
        <Width>7.78959in</Width>
        <ZIndex>2</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
          <TopBorder>
            <Color>Black</Color>
            <Style>Solid</Style>
            <Width>1pt</Width>
          </TopBorder>
          <BottomBorder>
            <Color>Black</Color>
            <Style>Solid</Style>
            <Width>1pt</Width>
          </BottomBorder>
          <PaddingLeft>2pt</PaddingLeft>
          <PaddingRight>2pt</PaddingRight>
          <PaddingTop>2pt</PaddingTop>
          <PaddingBottom>2pt</PaddingBottom>
        </Style>
      </Textbox>
      <Subreport Name=""CompanyDetail"">
        <ReportName>CompanyDetail</ReportName>
        <Parameters>
          <Parameter Name=""SiteId"">
            <Value>=First(Fields!SiteId.Value, ""DsMain"")</Value>
          </Parameter>
          <Parameter Name=""DivisionId"">
            <Value>=First(Fields!DivisionId.Value, ""DsMain"")</Value>
          </Parameter>
          <Parameter Name=""DatabaseConnectionString"">
            <Value>=Parameters!DatabaseConnectionString.Value</Value>
          </Parameter>
          <Parameter Name=""PrintedBy"">
            <Value>=Parameters!PrintedBy.Value</Value>
          </Parameter>
        </Parameters>
        <Top>0.03194in</Top>
        <Left>0.02685in</Left>
        <Height>0.12748in</Height>
        <Width>7.78146in</Width>
        <ZIndex>3</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Subreport>
      <Tablix Name=""Tablix6"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.23264in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1.59954in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.69658in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.65973in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.57638in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.41283in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.64911in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.48245in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.42169in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.64898in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.80667in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.5775in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.34375in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox31"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Sr</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox31</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox33"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!ProductCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox33</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox140"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=max(fields!SalesTaxProductCodeCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox140</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Dimension1Caption5"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!ContraDocTypeCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Dimension1Caption</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox69"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Qty</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox69</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox71"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Unit</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox71</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox34"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DealQtyCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox33</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox21"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Deal </Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Unit</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox21</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox73"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Rate</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox73</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox75"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Amount</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox75</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox153"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Remark</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox153</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox156"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!SalesTaxGroupProductCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox156</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.22917in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox32"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=RowNumber(Nothing)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox32</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ProductName"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Code.PrintProduct(Fields!ProductName.Value,Fields!ProductGroupCaption.Value,fields!ProductGroupName.Value,Fields!SpecificationCaption.Value,fields!Specification.Value,fields!Dimension1Caption.Value,fields!Dimension1Name.Value,fields!Dimension2Caption.Value,fields!Dimension2Name.Value,fields!Dimension3Caption.Value,fields!Dimension3Name.Value,fields!Dimension4Caption.Value,fields!Dimension4Name.Value,fields!ProductUidCaption.Value,fields!ProductUidName.Value)</Value>
                              <MarkupType>HTML</MarkupType>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ProductName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""SalesTaxProductCodes5"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!SalesTaxProductCodes.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>SalesTaxProductCodes5</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""PlanNo"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PlanNo.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>PlanNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Qty"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=iif(Fields!Qty.Value &lt;&gt; 0, formatnumber(Fields!Qty.Value,Fields!DecimalPlaces.Value),""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Qty</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""UnitName"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!UnitName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>UnitName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DealQty"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=iif(Fields!DealQty.Value &lt;&gt; 0, formatnumber(Fields!DealQty.Value,Fields!DealDecimalPlaces.Value),""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DealQty</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""DealUnitName"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!DealUnitName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>DealUnitName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Rate"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=iif(Fields!Rate.Value &lt;&gt; 0, formatnumber(Fields!Rate.Value,2),""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Rate</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Amount"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=iif(Fields!Amount.Value &lt;&gt; 0, formatnumber(Fields!Amount.Value,2),""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Amount</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox154"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Code.PrintLineRemark(fields!LineRemark.Value,fields!LotNo.Value,fields!DiscountPer.Value,Fields!DiscountAmt.Value,fields!LossQty.Value,fields!RecQty.Value)</Value>
                              <MarkupType>HTML</MarkupType>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox154</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox157"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!ChargeGroupProductName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox157</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.33333in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox78"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Total</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox78</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox142"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox142</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox86"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=iif(sum(Fields!Qty.Value) &lt;&gt; 0, formatnumber(sum(Fields!Qty.Value),max(Fields!DecimalPlaces.Value)),""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox86</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox29"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=iif(Sum(Fields!DealQty.Value) &lt;&gt; 0, formatnumber(sum(Fields!DealQty.Value),max(Fields!DealDecimalPlaces.Value)),""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox29</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox24"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox24</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox102"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=iif(Sum(Fields!Amount.Value) &lt;&gt; 0, formatnumber(sum(Fields!Amount.Value),2),""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox102</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox155"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox155</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox158"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox158</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
              <RepeatOnNewPage>true</RepeatOnNewPage>
            </TablixMember>
            <TablixMember>
              <Group Name=""Details"" />
            </TablixMember>
            <TablixMember />
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>2.32584in</Top>
        <Left>0.02204in</Left>
        <Height>0.90625in</Height>
        <Width>7.7641in</Width>
        <ZIndex>4</ZIndex>
        <Visibility>
          <Hidden>=iif(fields!DealUnitCnt.Value &lt;&gt; ""0"",false,true)</Hidden>
        </Visibility>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Subreport Name=""ProvisionalCompanyDetail"">
        <ReportName>ProvisionalCompanyDetail</ReportName>
        <Parameters>
          <Parameter Name=""SiteId"">
            <Value>=First(Fields!SiteId.Value, ""DsMain"")</Value>
          </Parameter>
          <Parameter Name=""DivisionId"">
            <Value>=First(Fields!DivisionId.Value, ""DsMain"")</Value>
          </Parameter>
          <Parameter Name=""DatabaseConnectionString"">
            <Value>=Parameters!DatabaseConnectionString.Value</Value>
          </Parameter>
          <Parameter Name=""PrintedBy"">
            <Value>=Parameters!PrintedBy.Value</Value>
          </Parameter>
        </Parameters>
        <Top>0.17362in</Top>
        <Left>0.03452in</Left>
        <Height>0.11428in</Height>
        <Width>7.77379in</Width>
        <ZIndex>5</ZIndex>
        <Visibility>
          <Hidden>true</Hidden>
        </Visibility>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Subreport>
      <Tablix Name=""Tablix13"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>2.80221in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>2.78336in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>2.1661in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.22917in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox12"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!SignatoryleftCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox6</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox13"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!SignatoryMiddleCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox7</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox59"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!SignatoryRightCaption.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox22</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember />
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>7.09041in</Top>
        <Left>0.03452in</Left>
        <Height>0.22917in</Height>
        <Width>7.75167in</Width>
        <ZIndex>6</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix7"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.29068in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>2.03258in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.7442in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.72818in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.55109in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.4917in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.55265in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.72785in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.98558in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.65637in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.34375in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox40"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Sr</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox31</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox41"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!ProductCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox33</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox141"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=max(fields!SalesTaxProductCodeCaption.Value)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox140</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Dimension1Caption6"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!ContraDocTypeCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Dimension1Caption</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox70"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Qty</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox69</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox72"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Unit</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox71</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox74"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Rate</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox73</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox77"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Amount</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox75</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox159"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Remark</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox153</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox160"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!SalesTaxGroupProductCaption.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox156</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.22917in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox42"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=RowNumber(Nothing)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox32</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ProductName2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Code.PrintProduct(Fields!ProductName.Value,Fields!ProductGroupCaption.Value,fields!ProductGroupName.Value,Fields!SpecificationCaption.Value,fields!Specification.Value,fields!Dimension1Caption.Value,fields!Dimension1Name.Value,fields!Dimension2Caption.Value,fields!Dimension2Name.Value,fields!Dimension3Caption.Value,fields!Dimension3Name.Value,fields!Dimension4Caption.Value,fields!Dimension4Name.Value,fields!ProductUidCaption.Value,fields!ProductUidName.Value)</Value>
                              <MarkupType>HTML</MarkupType>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ProductName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""SalesTaxProductCodes6"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!SalesTaxProductCodes.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>SalesTaxProductCodes5</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""PlanNo2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!PlanNo.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>PlanNo</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Qty2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!Qty.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Fields!Qty.Value, Fields!DecimalPlaces.Value), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Qty</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""UnitName2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!UnitName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>UnitName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Rate2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!Rate.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Fields!Rate.Value, 2), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Rate</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Amount2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!Amount.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Fields!Amount.Value, 2), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Amount</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox161"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Code.PrintLineRemark(fields!LineRemark.Value,fields!LotNo.Value,fields!DiscountPer.Value,Fields!DiscountAmt.Value,fields!LossQty.Value,fields!RecQty.Value)</Value>
                              <MarkupType>HTML</MarkupType>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox154</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox162"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=fields!ChargeGroupProductName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox157</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.33333in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox79"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Total</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox78</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox144"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox142</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox87"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=iif(Sum(Fields!Qty.Value) &lt;&gt; 0,formatnumber(Sum(Fields!Qty.Value), Max(Fields!DecimalPlaces.Value)), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox86</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox229"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox229</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox103"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Sum(Fields!Amount.Value) &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Sum(Fields!Amount.Value), 2), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox102</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox163"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox155</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox164"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8.5pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox158</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <VerticalAlign>Middle</VerticalAlign>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
              <RepeatOnNewPage>true</RepeatOnNewPage>
            </TablixMember>
            <TablixMember>
              <Group Name=""Details2"" />
            </TablixMember>
            <TablixMember />
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>3.26392in</Top>
        <Left>0.02532in</Left>
        <Height>0.90625in</Height>
        <Width>7.76088in</Width>
        <ZIndex>7</ZIndex>
        <Visibility>
          <Hidden>=iif(fields!DealUnitCnt.Value &lt;&gt; ""0"",true,false)</Hidden>
        </Visibility>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix14"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>4.34669in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.08681in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>3.29628in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>1.09678in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox254"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style />
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox254</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Rectangle Name=""Rectangle1"">
                      <ReportItems>
                        <Tablix Name=""Tablix8"">
                          <TablixBody>
                            <TablixColumns>
                              <TablixColumn>
                                <Width>1.18222in</Width>
                              </TablixColumn>
                              <TablixColumn>
                                <Width>0.03125in</Width>
                              </TablixColumn>
                              <TablixColumn>
                                <Width>1.96069in</Width>
                              </TablixColumn>
                            </TablixColumns>
                            <TablixRows>
                              <TablixRow>
                                <Height>0.22917in</Height>
                                <TablixCells>
                                  <TablixCell>
                                    <CellContents>
                                      <Textbox Name=""Textbox22"">
                                        <CanGrow>true</CanGrow>
                                        <KeepTogether>true</KeepTogether>
                                        <Paragraphs>
                                          <Paragraph>
                                            <TextRuns>
                                              <TextRun>
                                                <Value>- Return Amount  :</Value>
                                                <Style>
                                                  <FontFamily>Tahoma</FontFamily>
                                                  <FontSize>9pt</FontSize>
                                                </Style>
                                              </TextRun>
                                            </TextRuns>
                                            <Style />
                                          </Paragraph>
                                        </Paragraphs>
                                        <rd:DefaultName>Textbox1</rd:DefaultName>
                                        <Style>
                                          <Border>
                                            <Style>None</Style>
                                          </Border>
                                          <PaddingLeft>2pt</PaddingLeft>
                                          <PaddingRight>2pt</PaddingRight>
                                          <PaddingTop>2pt</PaddingTop>
                                          <PaddingBottom>2pt</PaddingBottom>
                                        </Style>
                                      </Textbox>
                                    </CellContents>
                                  </TablixCell>
                                  <TablixCell>
                                    <CellContents>
                                      <Textbox Name=""Textbox23"">
                                        <CanGrow>true</CanGrow>
                                        <KeepTogether>true</KeepTogether>
                                        <Paragraphs>
                                          <Paragraph>
                                            <TextRuns>
                                              <TextRun>
                                                <Value />
                                                <Style>
                                                  <FontFamily>Tahoma</FontFamily>
                                                  <FontSize>9pt</FontSize>
                                                </Style>
                                              </TextRun>
                                            </TextRuns>
                                            <Style />
                                          </Paragraph>
                                        </Paragraphs>
                                        <rd:DefaultName>Textbox5</rd:DefaultName>
                                        <Style>
                                          <Border>
                                            <Style>None</Style>
                                          </Border>
                                          <PaddingLeft>2pt</PaddingLeft>
                                          <PaddingRight>2pt</PaddingRight>
                                          <PaddingTop>2pt</PaddingTop>
                                          <PaddingBottom>2pt</PaddingBottom>
                                        </Style>
                                      </Textbox>
                                    </CellContents>
                                  </TablixCell>
                                  <TablixCell>
                                    <CellContents>
                                      <Textbox Name=""DebitNoteAmount"">
                                        <CanGrow>true</CanGrow>
                                        <KeepTogether>true</KeepTogether>
                                        <Paragraphs>
                                          <Paragraph>
                                            <TextRuns>
                                              <TextRun>
                                                <Value>=Fields!ReturnAmount.Value</Value>
                                                <Style>
                                                  <FontFamily>Tahoma</FontFamily>
                                                  <FontSize>9pt</FontSize>
                                                  <Format>0.00;(0.00);'-'</Format>
                                                </Style>
                                              </TextRun>
                                            </TextRuns>
                                            <Style>
                                              <TextAlign>Right</TextAlign>
                                            </Style>
                                          </Paragraph>
                                        </Paragraphs>
                                        <rd:DefaultName>DebitNoteAmount</rd:DefaultName>
                                        <Style>
                                          <Border>
                                            <Style>None</Style>
                                          </Border>
                                          <PaddingLeft>2pt</PaddingLeft>
                                          <PaddingRight>2pt</PaddingRight>
                                          <PaddingTop>2pt</PaddingTop>
                                          <PaddingBottom>2pt</PaddingBottom>
                                        </Style>
                                      </Textbox>
                                    </CellContents>
                                  </TablixCell>
                                </TablixCells>
                              </TablixRow>
                              <TablixRow>
                                <Height>0.25in</Height>
                                <TablixCells>
                                  <TablixCell>
                                    <CellContents>
                                      <Textbox Name=""Textbox48"">
                                        <CanGrow>true</CanGrow>
                                        <KeepTogether>true</KeepTogether>
                                        <Paragraphs>
                                          <Paragraph>
                                            <TextRuns>
                                              <TextRun>
                                                <Value>- Debit Amount  :</Value>
                                                <Style>
                                                  <FontFamily>Tahoma</FontFamily>
                                                  <FontSize>9pt</FontSize>
                                                </Style>
                                              </TextRun>
                                            </TextRuns>
                                            <Style />
                                          </Paragraph>
                                        </Paragraphs>
                                        <rd:DefaultName>Textbox48</rd:DefaultName>
                                        <Style>
                                          <Border>
                                            <Style>None</Style>
                                          </Border>
                                          <PaddingLeft>2pt</PaddingLeft>
                                          <PaddingRight>2pt</PaddingRight>
                                          <PaddingTop>2pt</PaddingTop>
                                          <PaddingBottom>2pt</PaddingBottom>
                                        </Style>
                                      </Textbox>
                                    </CellContents>
                                  </TablixCell>
                                  <TablixCell>
                                    <CellContents>
                                      <Textbox Name=""Textbox50"">
                                        <CanGrow>true</CanGrow>
                                        <KeepTogether>true</KeepTogether>
                                        <Paragraphs>
                                          <Paragraph>
                                            <TextRuns>
                                              <TextRun>
                                                <Value />
                                                <Style>
                                                  <FontFamily>Tahoma</FontFamily>
                                                  <FontSize>9pt</FontSize>
                                                </Style>
                                              </TextRun>
                                            </TextRuns>
                                            <Style />
                                          </Paragraph>
                                        </Paragraphs>
                                        <rd:DefaultName>Textbox50</rd:DefaultName>
                                        <Style>
                                          <Border>
                                            <Style>None</Style>
                                          </Border>
                                          <PaddingLeft>2pt</PaddingLeft>
                                          <PaddingRight>2pt</PaddingRight>
                                          <PaddingTop>2pt</PaddingTop>
                                          <PaddingBottom>2pt</PaddingBottom>
                                        </Style>
                                      </Textbox>
                                    </CellContents>
                                  </TablixCell>
                                  <TablixCell>
                                    <CellContents>
                                      <Textbox Name=""Textbox51"">
                                        <CanGrow>true</CanGrow>
                                        <KeepTogether>true</KeepTogether>
                                        <Paragraphs>
                                          <Paragraph>
                                            <TextRuns>
                                              <TextRun>
                                                <Value>=fields!DebitAmount.Value</Value>
                                                <Style>
                                                  <FontFamily>Tahoma</FontFamily>
                                                  <FontSize>9pt</FontSize>
                                                </Style>
                                              </TextRun>
                                            </TextRuns>
                                            <Style>
                                              <TextAlign>Right</TextAlign>
                                            </Style>
                                          </Paragraph>
                                        </Paragraphs>
                                        <rd:DefaultName>Textbox51</rd:DefaultName>
                                        <Style>
                                          <Border>
                                            <Style>None</Style>
                                          </Border>
                                          <PaddingLeft>2pt</PaddingLeft>
                                          <PaddingRight>2pt</PaddingRight>
                                          <PaddingTop>2pt</PaddingTop>
                                          <PaddingBottom>2pt</PaddingBottom>
                                        </Style>
                                      </Textbox>
                                    </CellContents>
                                  </TablixCell>
                                </TablixCells>
                              </TablixRow>
                              <TablixRow>
                                <Height>0.20833in</Height>
                                <TablixCells>
                                  <TablixCell>
                                    <CellContents>
                                      <Textbox Name=""Textbox25"">
                                        <CanGrow>true</CanGrow>
                                        <KeepTogether>true</KeepTogether>
                                        <Paragraphs>
                                          <Paragraph>
                                            <TextRuns>
                                              <TextRun>
                                                <Value>+ Credit Note  :</Value>
                                                <Style>
                                                  <FontFamily>Tahoma</FontFamily>
                                                  <FontSize>9pt</FontSize>
                                                </Style>
                                              </TextRun>
                                            </TextRuns>
                                            <Style />
                                          </Paragraph>
                                        </Paragraphs>
                                        <rd:DefaultName>Textbox2</rd:DefaultName>
                                        <Style>
                                          <Border>
                                            <Style>None</Style>
                                          </Border>
                                          <PaddingLeft>2pt</PaddingLeft>
                                          <PaddingRight>2pt</PaddingRight>
                                          <PaddingTop>2pt</PaddingTop>
                                          <PaddingBottom>2pt</PaddingBottom>
                                        </Style>
                                      </Textbox>
                                    </CellContents>
                                  </TablixCell>
                                  <TablixCell>
                                    <CellContents>
                                      <Textbox Name=""Textbox26"">
                                        <CanGrow>true</CanGrow>
                                        <KeepTogether>true</KeepTogether>
                                        <Paragraphs>
                                          <Paragraph>
                                            <TextRuns>
                                              <TextRun>
                                                <Value />
                                                <Style>
                                                  <FontFamily>Tahoma</FontFamily>
                                                  <FontSize>9pt</FontSize>
                                                </Style>
                                              </TextRun>
                                            </TextRuns>
                                            <Style />
                                          </Paragraph>
                                        </Paragraphs>
                                        <rd:DefaultName>Textbox6</rd:DefaultName>
                                        <Style>
                                          <Border>
                                            <Style>None</Style>
                                          </Border>
                                          <PaddingLeft>2pt</PaddingLeft>
                                          <PaddingRight>2pt</PaddingRight>
                                          <PaddingTop>2pt</PaddingTop>
                                          <PaddingBottom>2pt</PaddingBottom>
                                        </Style>
                                      </Textbox>
                                    </CellContents>
                                  </TablixCell>
                                  <TablixCell>
                                    <CellContents>
                                      <Textbox Name=""DebitNoteAmount4"">
                                        <CanGrow>true</CanGrow>
                                        <KeepTogether>true</KeepTogether>
                                        <Paragraphs>
                                          <Paragraph>
                                            <TextRuns>
                                              <TextRun>
                                                <Value>=Fields!CreaditAmount.Value</Value>
                                                <Style>
                                                  <FontFamily>Tahoma</FontFamily>
                                                  <FontSize>9pt</FontSize>
                                                  <Format>0.00;(0.00);'-'</Format>
                                                </Style>
                                              </TextRun>
                                            </TextRuns>
                                            <Style>
                                              <TextAlign>Right</TextAlign>
                                            </Style>
                                          </Paragraph>
                                        </Paragraphs>
                                        <rd:DefaultName>DebitNoteAmount</rd:DefaultName>
                                        <Style>
                                          <Border>
                                            <Style>None</Style>
                                          </Border>
                                          <PaddingLeft>2pt</PaddingLeft>
                                          <PaddingRight>2pt</PaddingRight>
                                          <PaddingTop>2pt</PaddingTop>
                                          <PaddingBottom>2pt</PaddingBottom>
                                        </Style>
                                      </Textbox>
                                    </CellContents>
                                  </TablixCell>
                                </TablixCells>
                              </TablixRow>
                              <TablixRow>
                                <Height>0.21875in</Height>
                                <TablixCells>
                                  <TablixCell>
                                    <CellContents>
                                      <Textbox Name=""Textbox28"">
                                        <CanGrow>true</CanGrow>
                                        <KeepTogether>true</KeepTogether>
                                        <Paragraphs>
                                          <Paragraph>
                                            <TextRuns>
                                              <TextRun>
                                                <Value>   Net Payable :</Value>
                                                <Style>
                                                  <FontFamily>Tahoma</FontFamily>
                                                  <FontSize>9pt</FontSize>
                                                  <FontWeight>Bold</FontWeight>
                                                </Style>
                                              </TextRun>
                                            </TextRuns>
                                            <Style>
                                              <TextAlign>Left</TextAlign>
                                            </Style>
                                          </Paragraph>
                                        </Paragraphs>
                                        <rd:DefaultName>Textbox14</rd:DefaultName>
                                        <Style>
                                          <Border>
                                            <Style>None</Style>
                                          </Border>
                                          <TopBorder>
                                            <Style>Solid</Style>
                                          </TopBorder>
                                          <BottomBorder>
                                            <Style>Solid</Style>
                                          </BottomBorder>
                                          <PaddingLeft>2pt</PaddingLeft>
                                          <PaddingRight>2pt</PaddingRight>
                                          <PaddingTop>2pt</PaddingTop>
                                          <PaddingBottom>2pt</PaddingBottom>
                                        </Style>
                                      </Textbox>
                                    </CellContents>
                                  </TablixCell>
                                  <TablixCell>
                                    <CellContents>
                                      <Textbox Name=""Textbox30"">
                                        <CanGrow>true</CanGrow>
                                        <KeepTogether>true</KeepTogether>
                                        <Paragraphs>
                                          <Paragraph>
                                            <TextRuns>
                                              <TextRun>
                                                <Value />
                                                <Style>
                                                  <FontFamily>Tahoma</FontFamily>
                                                  <FontSize>9pt</FontSize>
                                                  <FontWeight>Bold</FontWeight>
                                                </Style>
                                              </TextRun>
                                            </TextRuns>
                                            <Style />
                                          </Paragraph>
                                        </Paragraphs>
                                        <rd:DefaultName>Textbox20</rd:DefaultName>
                                        <Style>
                                          <Border>
                                            <Style>None</Style>
                                          </Border>
                                          <TopBorder>
                                            <Color>Black</Color>
                                            <Style>Solid</Style>
                                            <Width>1pt</Width>
                                          </TopBorder>
                                          <BottomBorder>
                                            <Color>Black</Color>
                                            <Style>Solid</Style>
                                            <Width>1pt</Width>
                                          </BottomBorder>
                                          <PaddingLeft>2pt</PaddingLeft>
                                          <PaddingRight>2pt</PaddingRight>
                                          <PaddingTop>2pt</PaddingTop>
                                          <PaddingBottom>2pt</PaddingBottom>
                                        </Style>
                                      </Textbox>
                                    </CellContents>
                                  </TablixCell>
                                  <TablixCell>
                                    <CellContents>
                                      <Textbox Name=""NetPayableAmount"">
                                        <CanGrow>true</CanGrow>
                                        <KeepTogether>true</KeepTogether>
                                        <Paragraphs>
                                          <Paragraph>
                                            <TextRuns>
                                              <TextRun>
                                                <Value>=Fields!NetAmount.Value</Value>
                                                <Style>
                                                  <FontFamily>Tahoma</FontFamily>
                                                  <FontSize>9pt</FontSize>
                                                  <FontWeight>Bold</FontWeight>
                                                  <Format>0.00;(0.00);'-'</Format>
                                                </Style>
                                              </TextRun>
                                            </TextRuns>
                                            <Style>
                                              <TextAlign>Right</TextAlign>
                                            </Style>
                                          </Paragraph>
                                        </Paragraphs>
                                        <rd:DefaultName>NetPayableAmount</rd:DefaultName>
                                        <Style>
                                          <Border>
                                            <Style>None</Style>
                                          </Border>
                                          <TopBorder>
                                            <Color>Black</Color>
                                            <Style>Solid</Style>
                                            <Width>1pt</Width>
                                          </TopBorder>
                                          <BottomBorder>
                                            <Color>Black</Color>
                                            <Style>Solid</Style>
                                            <Width>1pt</Width>
                                          </BottomBorder>
                                          <PaddingLeft>2pt</PaddingLeft>
                                          <PaddingRight>2pt</PaddingRight>
                                          <PaddingTop>2pt</PaddingTop>
                                          <PaddingBottom>2pt</PaddingBottom>
                                        </Style>
                                      </Textbox>
                                    </CellContents>
                                  </TablixCell>
                                </TablixCells>
                              </TablixRow>
                            </TablixRows>
                          </TablixBody>
                          <TablixColumnHierarchy>
                            <TablixMembers>
                              <TablixMember />
                              <TablixMember />
                              <TablixMember />
                            </TablixMembers>
                          </TablixColumnHierarchy>
                          <TablixRowHierarchy>
                            <TablixMembers>
                              <TablixMember>
                                <Visibility>
                                  <Hidden>=fields!ReturnAmount.Value=0</Hidden>
                                </Visibility>
                              </TablixMember>
                              <TablixMember>
                                <Visibility>
                                  <Hidden>=fields!DebitAmount.Value=0</Hidden>
                                </Visibility>
                              </TablixMember>
                              <TablixMember>
                                <Visibility>
                                  <Hidden>=fields!CreaditAmount.Value=0</Hidden>
                                </Visibility>
                              </TablixMember>
                              <TablixMember>
                                <Visibility>
                                  <Hidden>=iif(fields!ReturnAmount.Value &lt;&gt; 0 or fields!DebitAmount.Value &lt;&gt; 0 or fields!CreaditAmount.Value &lt;&gt; 0,false,true)</Hidden>
                                </Visibility>
                              </TablixMember>
                            </TablixMembers>
                          </TablixRowHierarchy>
                          <DataSetName>DsMain</DataSetName>
                          <Top>0.04469in</Top>
                          <Left>0.03236in</Left>
                          <Height>0.90625in</Height>
                          <Width>3.17416in</Width>
                          <Style>
                            <Border>
                              <Style>None</Style>
                            </Border>
                          </Style>
                        </Tablix>
                      </ReportItems>
                      <KeepTogether>true</KeepTogether>
                      <ZIndex>9</ZIndex>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                      </Style>
                    </Rectangle>
                    <ColSpan>2</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.04688in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox54"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Dyeing Loss %  : </Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                            <TextRun>
                              <Value>=Round(Sum(Fields!LossQty.Value)*100/Sum(Fields!Qty.Value),2)</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox54</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox55"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style />
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox55</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox56"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style />
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox56</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.14844in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox218"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=""Amount in Words : "" + ""( "" + Code.RupeesToWord(Max(Fields!NetAmount.Value, ""DsMain""))+"")""</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox218</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>3</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.08594in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox1"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox1</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox16"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox16</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox2</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.10677in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox46"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Remark : </Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                            <TextRun>
                              <Value>=Fields!Remark.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox40</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>3</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.10677in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox95"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>FOR </Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                            <TextRun>
                              <Value>=fields!CompanyName.Value</Value>
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox35</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>3</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.22395in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox226"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox226</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox17"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox17</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox15"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox15</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.03125in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox232"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox232</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox20"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox20</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox233"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox233</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember>
              <Visibility>
                <Hidden>=iif(Max(Fields!NetAmount.Value, ""DsMain"")&lt;&gt; 0,false,true)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember />
            <TablixMember>
              <Visibility>
                <Hidden>=iif(fields!Remark.Value &lt;&gt; """",false,true)</Hidden>
              </Visibility>
            </TablixMember>
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsMain</DataSetName>
        <Top>5.26222in</Top>
        <Left>0.02536in</Left>
        <Height>1.84677in</Height>
        <Width>7.72978in</Width>
        <ZIndex>8</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix9"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.92969in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.68402in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.61728in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.92969in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ChargeName2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!ChargeName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>=iif(Fields!ChargeName.Value=""Net Amount"",""Bold"",""Normal"")</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ChargeName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>=iif(Fields!ChargeName.Value=""Net Amount"",""Solid"",""Default"")</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>=iif(Fields!ChargeName.Value=""Net Amount"",""Solid"",""Default"")</Style>
                        </BottomBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox5"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!Rate.Value &lt;&gt; 0, "" @ "", """")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>=iif(Fields!ChargeName.Value=""Net Amount"",""Bold"",""Normal"")</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox5</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>=iif(Fields!ChargeName.Value=""Net Amount"",""Solid"",""Default"")</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>=iif(Fields!ChargeName.Value=""Net Amount"",""Solid"",""Default"")</Style>
                        </BottomBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Rate3"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!Rate.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.ForMatNumber(Fields!Rate.Value, 2) + "" %"", """")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>=iif(Fields!ChargeName.Value=""Net Amount"",""Bold"",""Normal"")</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Rate</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>=iif(Fields!ChargeName.Value=""Net Amount"",""Solid"",""Default"")</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>=iif(Fields!ChargeName.Value=""Net Amount"",""Solid"",""Default"")</Style>
                        </BottomBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""OrderAmount"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Strings.formatnumber(Fields!Amount.Value, 2)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>=iif(Fields!ChargeName.Value=""Net Amount"",""Bold"",""Normal"")</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>OrderAmount</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>=iif(Fields!ChargeName.Value=""Net Amount"",""Solid"",""Default"")</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>=iif(Fields!ChargeName.Value=""Net Amount"",""Solid"",""Default"")</Style>
                        </BottomBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox35"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Net Amount</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox1</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox37"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox6</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox38"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value />
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Left</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox2</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""OrderAmount2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Strings.formatnumber(Sum(Fields!Amount.Value), 2)</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>9pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>OrderAmount</rd:DefaultName>
                      <Style>
                        <Border>
                          <Style>None</Style>
                        </Border>
                        <TopBorder>
                          <Style>Solid</Style>
                        </TopBorder>
                        <BottomBorder>
                          <Style>Solid</Style>
                        </BottomBorder>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember>
              <Group Name=""Details1"" />
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>Before</KeepWithGroup>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsCalculation</DataSetName>
        <Top>4.23961in</Top>
        <Left>4.64764in</Left>
        <Height>0.5in</Height>
        <Width>3.16067in</Width>
        <ZIndex>9</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
      <Tablix Name=""Tablix11"">
        <TablixBody>
          <TablixColumns>
            <TablixColumn>
              <Width>0.69721in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.7833in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.60627in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.6262in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>0.73958in</Width>
            </TablixColumn>
            <TablixColumn>
              <Width>1in</Width>
            </TablixColumn>
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox43"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>GST Summary</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox29</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                    <ColSpan>6</ColSpan>
                  </CellContents>
                </TablixCell>
                <TablixCell />
                <TablixCell />
                <TablixCell />
                <TablixCell />
                <TablixCell />
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox44"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Desc</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox3</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox45"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Taxable </Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox7</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox47"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>IGST</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox9</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox53"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>CGST</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox11</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox57"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>SGST</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox13</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox58"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value> Cess</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>8pt</FontSize>
                                <FontWeight>Bold</FontWeight>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox15</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>LightGrey</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""ChargeGroupProductName2"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Fields!ChargeGroupProductName.Value</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>7.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>ChargeGroupProductName</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""TaxableAmount"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!TaxableAmount.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Fields!TaxableAmount.Value, 2), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>7.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>TaxableAmount</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""IGST"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!IGST.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Fields!IGST.Value, 2), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>7.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>IGST</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""CGST"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!CGST.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Fields!CGST.Value, 2), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>7.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>CGST</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""SGST"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!SGST.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Fields!SGST.Value, 2), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>7.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>SGST</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""GSTCess"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Fields!GSTCess.Value &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Fields!GSTCess.Value, 2), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>7.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>GSTCess</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox60"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>Total</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>7.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style />
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox17</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                        </TopBorder>
                        <BottomBorder>
                          <Color>Black</Color>
                        </BottomBorder>
                        <LeftBorder>
                          <Style>None</Style>
                        </LeftBorder>
                        <RightBorder>
                          <Style>None</Style>
                        </RightBorder>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox61"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Sum(Fields!TaxableAmount.Value) &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Sum(Fields!TaxableAmount.Value), 2), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>7.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox18</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                        </TopBorder>
                        <BottomBorder>
                          <Color>Black</Color>
                        </BottomBorder>
                        <LeftBorder>
                          <Style>None</Style>
                        </LeftBorder>
                        <RightBorder>
                          <Style>None</Style>
                        </RightBorder>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox62"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Sum(Fields!IGST.Value) &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Sum(Fields!IGST.Value), 2), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>7.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox19</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                        </TopBorder>
                        <BottomBorder>
                          <Color>Black</Color>
                        </BottomBorder>
                        <LeftBorder>
                          <Style>None</Style>
                        </LeftBorder>
                        <RightBorder>
                          <Style>None</Style>
                        </RightBorder>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox63"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Sum(Fields!CGST.Value) &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Sum(Fields!CGST.Value), 2), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>7.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox20</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                        </TopBorder>
                        <BottomBorder>
                          <Color>Black</Color>
                        </BottomBorder>
                        <LeftBorder>
                          <Style>None</Style>
                        </LeftBorder>
                        <RightBorder>
                          <Style>None</Style>
                        </RightBorder>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox64"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Sum(Fields!SGST.Value) &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Sum(Fields!SGST.Value), 2), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>7.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox21</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                        </TopBorder>
                        <BottomBorder>
                          <Color>Black</Color>
                        </BottomBorder>
                        <LeftBorder>
                          <Style>None</Style>
                        </LeftBorder>
                        <RightBorder>
                          <Style>None</Style>
                        </RightBorder>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
                <TablixCell>
                  <CellContents>
                    <Textbox Name=""Textbox65"">
                      <CanGrow>true</CanGrow>
                      <KeepTogether>true</KeepTogether>
                      <Paragraphs>
                        <Paragraph>
                          <TextRuns>
                            <TextRun>
                              <Value>=Microsoft.VisualBasic.Interaction.iif(Sum(Fields!GSTCess.Value) &lt;&gt; 0, Microsoft.VisualBasic.Strings.formatnumber(Sum(Fields!GSTCess.Value), 2), ""-"")</Value>
                              <Style>
                                <FontFamily>Tahoma</FontFamily>
                                <FontSize>7.5pt</FontSize>
                              </Style>
                            </TextRun>
                          </TextRuns>
                          <Style>
                            <TextAlign>Right</TextAlign>
                          </Style>
                        </Paragraph>
                      </Paragraphs>
                      <rd:DefaultName>Textbox22</rd:DefaultName>
                      <Style>
                        <Border>
                          <Color>LightGrey</Color>
                          <Style>Solid</Style>
                        </Border>
                        <TopBorder>
                          <Color>Black</Color>
                        </TopBorder>
                        <BottomBorder>
                          <Color>Black</Color>
                        </BottomBorder>
                        <LeftBorder>
                          <Style>None</Style>
                        </LeftBorder>
                        <RightBorder>
                          <Style>None</Style>
                        </RightBorder>
                        <BackgroundColor>=iif(RowNumber(nothing) Mod 2 , ""White"",""WhiteSmoke"")</BackgroundColor>
                        <PaddingLeft>2pt</PaddingLeft>
                        <PaddingRight>2pt</PaddingRight>
                        <PaddingTop>2pt</PaddingTop>
                        <PaddingBottom>2pt</PaddingBottom>
                      </Style>
                    </Textbox>
                  </CellContents>
                </TablixCell>
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
            <TablixMember />
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
            </TablixMember>
            <TablixMember>
              <Group Name=""Details3"" />
              <TablixMembers>
                <TablixMember />
                <TablixMember />
              </TablixMembers>
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>DsGSTSummary</DataSetName>
        <Top>4.23961in</Top>
        <Left>0.12375in</Left>
        <Height>1in</Height>
        <Width>4.45256in</Width>
        <ZIndex>10</ZIndex>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
    </ReportItems>
    <Height>7.36013in</Height>
    <Style />
  </Body>
  <Width>7.81549in</Width>
  <Page>
    <PageHeader>
      <Height>0.58333in</Height>
      <PrintOnFirstPage>true</PrintOnFirstPage>
      <PrintOnLastPage>true</PrintOnLastPage>
      <ReportItems>
        <Textbox Name=""ReportTitle3"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>=First(Fields!ReportTitle.Value, ""DsMain"")</Value>
                  <Style>
                    <FontFamily>Tahoma</FontFamily>
                    <FontSize>14pt</FontSize>
                    <FontWeight>Bold</FontWeight>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style>
                <TextAlign>Center</TextAlign>
              </Style>
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>ReportTitle</rd:DefaultName>
          <Top>0.02569in</Top>
          <Left>0.0413in</Left>
          <Height>0.25in</Height>
          <Width>7.74489in</Width>
          <Visibility>
            <Hidden>=iif(Globals!PageNumber=1,True,False)</Hidden>
          </Visibility>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <TopBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </TopBorder>
            <BottomBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </BottomBorder>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox4"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>=First(Fields!PartyCaption.Value, ""DsMain"")+"" : "" + First(Fields!PartyName.Value, ""DsMain"")</Value>
                  <Style>
                    <FontFamily>Tahoma</FontFamily>
                    <FontSize>9pt</FontSize>
                    <FontWeight>Bold</FontWeight>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style />
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox4</rd:DefaultName>
          <Top>0.31597in</Top>
          <Left>0.0538in</Left>
          <Height>0.25in</Height>
          <Width>3.83417in</Width>
          <ZIndex>1</ZIndex>
          <Visibility>
            <Hidden>=iif(Globals!PageNumber=1,True,False)</Hidden>
          </Visibility>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <BottomBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </BottomBorder>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox8"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>=""Order No : "" + First(Fields!DocNo.Value, ""DsMain"")</Value>
                  <Style>
                    <FontFamily>Tahoma</FontFamily>
                    <FontSize>9pt</FontSize>
                    <FontWeight>Bold</FontWeight>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style />
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox4</rd:DefaultName>
          <Top>0.31597in</Top>
          <Left>3.92123in</Left>
          <Height>0.25in</Height>
          <Width>3.86496in</Width>
          <ZIndex>2</ZIndex>
          <Visibility>
            <Hidden>=iif(Globals!PageNumber=1,True,False)</Hidden>
          </Visibility>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <BottomBorder>
              <Color>Black</Color>
              <Style>Solid</Style>
              <Width>1pt</Width>
            </BottomBorder>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
      </ReportItems>
      <Style>
        <Border>
          <Style>None</Style>
        </Border>
      </Style>
    </PageHeader>
    <PageFooter>
      <Height>0.31766in</Height>
      <PrintOnFirstPage>true</PrintOnFirstPage>
      <PrintOnLastPage>true</PrintOnLastPage>
      <ReportItems>
        <Textbox Name=""Textbox18"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>Page </Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value>=Globals!PageNumber</Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value> of </Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value>=Globals!TotalPages</Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style>
                <TextAlign>Right</TextAlign>
              </Style>
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox18</rd:DefaultName>
          <Top>0.03849in</Top>
          <Left>5.1813in</Left>
          <Height>0.22917in</Height>
          <Width>2.57384in</Width>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox19"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>Print Date : </Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                    <Format>dd-MMM-yy hh:mm:ss tt</Format>
                  </Style>
                </TextRun>
                <TextRun>
                  <Value>=Globals!ExecutionTime</Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>8pt</FontSize>
                    <Format>dd-MMM-yy hh:mm:ss tt</Format>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style>
                <TextAlign>Left</TextAlign>
              </Style>
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox19</rd:DefaultName>
          <Top>0.06944in</Top>
          <Left>0.0788in</Left>
          <Height>0.1875in</Height>
          <Width>2.52546in</Width>
          <ZIndex>1</ZIndex>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
        <Textbox Name=""Textbox98"">
          <CanGrow>true</CanGrow>
          <KeepTogether>true</KeepTogether>
          <Paragraphs>
            <Paragraph>
              <TextRuns>
                <TextRun>
                  <Value>=""Printed By : "" + Parameters!PrintedBy.Value</Value>
                  <Style>
                    <FontFamily>tahoma</FontFamily>
                    <FontSize>9pt</FontSize>
                    <Format>dd-MMM-yy hh:mm:ss tt</Format>
                  </Style>
                </TextRun>
              </TextRuns>
              <Style>
                <TextAlign>Center</TextAlign>
              </Style>
            </Paragraph>
          </Paragraphs>
          <rd:DefaultName>Textbox19</rd:DefaultName>
          <Top>0.06195in</Top>
          <Left>2.85186in</Left>
          <Height>0.1875in</Height>
          <Width>2.12468in</Width>
          <ZIndex>2</ZIndex>
          <Style>
            <Border>
              <Style>None</Style>
            </Border>
            <PaddingLeft>2pt</PaddingLeft>
            <PaddingRight>2pt</PaddingRight>
            <PaddingTop>2pt</PaddingTop>
            <PaddingBottom>2pt</PaddingBottom>
          </Style>
        </Textbox>
      </ReportItems>
      <Style>
        <Border>
          <Style>None</Style>
        </Border>
      </Style>
    </PageFooter>
    <PageHeight>11.69in</PageHeight>
    <PageWidth>8.27in</PageWidth>
    <LeftMargin>0.25in</LeftMargin>
    <TopMargin>0.1in</TopMargin>
    <BottomMargin>0.1in</BottomMargin>
    <Style />
  </Page>
  <AutoRefresh>0</AutoRefresh>
  <DataSources>
    <DataSource Name=""DyeingCancelPrint"">
      <ConnectionProperties>
        <DataProvider>SQL</DataProvider>
        <ConnectString>=Parameters!DatabaseConnectionString.Value</ConnectString>
      </ConnectionProperties>
      <rd:SecurityType>None</rd:SecurityType>
      <rd:DataSourceID>727bb65e-cc51-4d28-a097-a8ea7d087931</rd:DataSourceID>
    </DataSource>
  </DataSources>
  <DataSets>
    <DataSet Name=""DsMain"">
      <Query>
        <DataSourceName>DyeingCancelPrint</DataSourceName>
        <QueryParameters>
          <QueryParameter Name=""@Id"">
            <Value>=Parameters!Id.Value</Value>
          </QueryParameter>
        </QueryParameters>
        <CommandText>Web.StdDocPrint_JobInvoice</CommandText>
      </Query>
      <Fields>
        <Field Name=""JobInvoiceHeaderId"">
          <DataField>JobInvoiceHeaderId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""DocTypeId"">
          <DataField>DocTypeId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""DocNo"">
          <DataField>DocNo</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SiteId"">
          <DataField>SiteId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""DivisionId"">
          <DataField>DivisionId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""DocDate"">
          <DataField>DocDate</DataField>
          <rd:TypeName>System.DateTime</rd:TypeName>
        </Field>
        <Field Name=""PartyDocNo"">
          <DataField>PartyDocNo</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""CreditDays"">
          <DataField>CreditDays</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""Remark"">
          <DataField>Remark</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyDocDate"">
          <DataField>PartyDocDate</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyDocCaption"">
          <DataField>PartyDocCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""DocumentTypeShortName"">
          <DataField>DocumentTypeShortName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyDocDateCaption"">
          <DataField>PartyDocDateCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ModifiedBy"">
          <DataField>ModifiedBy</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ModifiedDate"">
          <DataField>ModifiedDate</DataField>
          <rd:TypeName>System.DateTime</rd:TypeName>
        </Field>
        <Field Name=""Status"">
          <DataField>Status</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""CurrencyName"">
          <DataField>CurrencyName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ReverseCharge"">
          <DataField>ReverseCharge</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyName"">
          <DataField>PartyName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyCaption"">
          <DataField>PartyCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartySuffix"">
          <DataField>PartySuffix</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyAddress"">
          <DataField>PartyAddress</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyStateName"">
          <DataField>PartyStateName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyMobileNo"">
          <DataField>PartyMobileNo</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PartyStateCode"">
          <DataField>PartyStateCode</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""CustomerId"">
          <DataField>CustomerId</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""Party_TIN_NO"">
          <DataField>Party TIN NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Party_PAN_NO"">
          <DataField>Party PAN NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Party_AADHAR_NO"">
          <DataField>Party AADHAR NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Party_GST_NO"">
          <DataField>Party GST NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Party_CST_NO"">
          <DataField>Party CST NO</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""PlanNo"">
          <DataField>PlanNo</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ContraDocTypeCaption"">
          <DataField>ContraDocTypeCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SignatoryMiddleCaption"">
          <DataField>SignatoryMiddleCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SignatoryRightCaption"">
          <DataField>SignatoryRightCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SpecificationCaption"">
          <DataField>SpecificationCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductName"">
          <DataField>ProductName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductCaption"">
          <DataField>ProductCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""UnitName"">
          <DataField>UnitName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""DecimalPlaces"">
          <DataField>DecimalPlaces</DataField>
          <rd:TypeName>System.Byte</rd:TypeName>
        </Field>
        <Field Name=""DealUnitName"">
          <DataField>DealUnitName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""DealQtyCaption"">
          <DataField>DealQtyCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""DealDecimalPlaces"">
          <DataField>DealDecimalPlaces</DataField>
          <rd:TypeName>System.Byte</rd:TypeName>
        </Field>
        <Field Name=""Qty"">
          <DataField>Qty</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""Rate"">
          <DataField>Rate</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""Amount"">
          <DataField>Amount</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""DealQty"">
          <DataField>DealQty</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""Dimension1Name"">
          <DataField>Dimension1Name</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension1Caption"">
          <DataField>Dimension1Caption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension2Name"">
          <DataField>Dimension2Name</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension2Caption"">
          <DataField>Dimension2Caption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension3Name"">
          <DataField>Dimension3Name</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension3Caption"">
          <DataField>Dimension3Caption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension4Name"">
          <DataField>Dimension4Name</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Dimension4Caption"">
          <DataField>Dimension4Caption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""LotNo"">
          <DataField>LotNo</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""Specification"">
          <DataField>Specification</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SignatoryleftCaption"">
          <DataField>SignatoryleftCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""LineRemark"">
          <DataField>LineRemark</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""SalesTaxProductCodes"">
          <DataField>SalesTaxProductCodes</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""LossQty"">
          <DataField>LossQty</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""ProductGroupName"">
          <DataField>ProductGroupName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductGroupCaption"">
          <DataField>ProductGroupCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""RecQty"">
          <DataField>RecQty</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""ChargeGroupProductName"">
          <DataField>ChargeGroupProductName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ProductUidCaption"">
          <DataField>ProductUidCaption</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""NetAmount"">
          <DataField>NetAmount</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""ChargeGroupPersonName"">
          <DataField>ChargeGroupPersonName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""DealUnitCnt"">
          <DataField>DealUnitCnt</DataField>
          <rd:TypeName>System.Int32</rd:TypeName>
        </Field>
        <Field Name=""SubReportProcList"">
          <DataField>SubReportProcList</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ReportTitle"">
          <DataField>ReportTitle</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ReportName"">
          <DataField>ReportName</DataField>
          <rd:TypeName>System.String</rd:TypeName>
        </Field>
        <Field Name=""ReturnAmount"">
          <DataField>ReturnAmount</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""CompanyName"">
          <DataField>CompanyName</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""DebitAmount"">
          <DataField>DebitAmount</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""DocIdCaption"">
          <DataField>DocIdCaption</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""CreaditAmount"">
          <DataField>CreaditAmount</DataField>
          <rd:TypeName>System.Decimal</rd:TypeName>
        </Field>
        <Field Name=""DocIdCaptionDate"">
          <DataField>DocIdCaptionDate</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""DocIdCaptionDueDate"">
          <DataField>DocIdCaptionDueDate</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""DiscountPer"">
          <DataField>DiscountPer</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""DiscountAmt"">
          <DataField>DiscountAmt</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""SalesTaxGroupProductCaption"">
          <DataField>SalesTaxGroupProductCaption</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""ProductUidName"">
          <DataField>ProductUidName</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""SalesTaxProductCodeCaption"">
          <DataField>SalesTaxProductCodeCaption</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
      </Fields>
    </DataSet>
    <DataSet Name=""DsCalculation"">
      <Query>
        <DataSourceName>DyeingCancelPrint</DataSourceName>
        <CommandText />
      </Query>
      <Fields>
        <Field Name=""Id"">
          <DataField>Id</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""HeaderTableId"">
          <DataField>HeaderTableId</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""Sr"">
          <DataField>Sr</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""ChargeName"">
          <DataField>ChargeName</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""Amount"">
          <DataField>Amount</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""ChargeTypeId"">
          <DataField>ChargeTypeId</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""ChargeTypeName"">
          <DataField>ChargeTypeName</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""Rate"">
          <DataField>Rate</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""ReportName"">
          <DataField>ReportName</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
      </Fields>
    </DataSet>
    <DataSet Name=""DsGSTSummary"">
      <Query>
        <DataSourceName>DyeingCancelPrint</DataSourceName>
        <CommandText />
      </Query>
      <Fields>
        <Field Name=""ChargeGroupProductName"">
          <DataField>ChargeGroupProductName</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""TaxableAmount"">
          <DataField>TaxableAmount</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""IGST"">
          <DataField>IGST</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""CGST"">
          <DataField>CGST</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""SGST"">
          <DataField>SGST</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
        <Field Name=""GSTCess"">
          <DataField>GSTCess</DataField>
          <rd:UserDefined>true</rd:UserDefined>
        </Field>
      </Fields>
    </DataSet>
  </DataSets>
  <ReportParameters>
    <ReportParameter Name=""Id"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>Id</Prompt>
    </ReportParameter>
    <ReportParameter Name=""PrintedBy"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>PrintedBy</Prompt>
    </ReportParameter>
    <ReportParameter Name=""DatabaseConnectionString"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>DatabaseConnectionString</Prompt>
    </ReportParameter>
    <ReportParameter Name=""CompanyName"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>CompanyName</Prompt>
    </ReportParameter>
    <ReportParameter Name=""FilterStr1"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>FilterStr1</Prompt>
    </ReportParameter>
    <ReportParameter Name=""FilterStr2"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>FilterStr2</Prompt>
    </ReportParameter>
    <ReportParameter Name=""FilterStr3"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>FilterStr3</Prompt>
    </ReportParameter>
    <ReportParameter Name=""FilterStr4"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>FilterStr4</Prompt>
    </ReportParameter>
    <ReportParameter Name=""FilterStr5"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>FilterStr5</Prompt>
    </ReportParameter>
    <ReportParameter Name=""FilterStr6"">
      <DataType>String</DataType>
      <Nullable>true</Nullable>
      <AllowBlank>true</AllowBlank>
      <Prompt>FilterStr6</Prompt>
    </ReportParameter>
  </ReportParameters>
  <Code>
    Function RupeesToWord(ByVal MyNumber)
    Dim Temp
    Dim Rupees, Paisa As String
    Dim DecimalPlace, iCount
    Dim Hundreds, Words As String
    Dim place(9) As String
    place(0) = "" Thousand ""
    place(2) = "" Lakh ""
    place(4) = "" Crore ""
    place(6) = "" Arab ""
    place(8) = "" Kharab ""
    On Error Resume Next
    ' Convert MyNumber to a string, trimming extra spaces.
    MyNumber = Trim(Str(MyNumber))

    ' Find decimal place.
    DecimalPlace = InStr(MyNumber, ""."") 

    ' If we find decimal place...
    If DecimalPlace &gt; 0 Then
    ' Convert Paisa
    Temp = Left(Mid(MyNumber, DecimalPlace + 1) &amp; ""00"", 2)
    'Paisa = "" and "" &amp; ConvertTens(Temp) &amp; "" Paisa""
    Paisa = """"

    ' Strip off paisa from remainder to convert.
    MyNumber = Trim(Left(MyNumber, DecimalPlace - 1))
    End If

    '===============================================================
    Dim TM As String ' If MyNumber between Rs.1 To 99 Only.
    TM = Right(MyNumber, 2)

    If Len(MyNumber) &gt; 0 And Len(MyNumber) &lt;= 2 Then
    If Len(TM) = 1 Then
    Words = ConvertDigit(TM)
    RupeesToWord = Words &amp; Paisa

    Exit Function

    Else
    If Len(TM) = 2 Then
    Words = ConvertTens(TM)
    RupeesToWord = Words &amp; Paisa
    Exit Function

    End If
    End If
    End If
    '===============================================================


    ' Convert last 3 digits of MyNumber to ruppees in word.
    Hundreds = ConvertHundreds(Right(MyNumber, 3))
    ' Strip off last three digits
    MyNumber = Left(MyNumber, Len(MyNumber) - 3)

    iCount = 0
    Do While MyNumber &lt;&gt; """"
    'Strip last two digits
    Temp = Right(MyNumber, 2)
    If Len(MyNumber) = 1 Then


    If Trim(Words) = ""Thousand"" Or _
    Trim(Words) = ""Lakh Thousand"" Or _
    Trim(Words) = ""Lakh"" Or _
    Trim(Words) = ""Crore"" Or _
    Trim(Words) = ""Crore Lakh Thousand"" Or _
    Trim(Words) = ""Arab Crore Lakh Thousand"" Or _
    Trim(Words) = ""Arab"" Or _
    Trim(Words) = ""Kharab Arab Crore Lakh Thousand"" Or _
    Trim(Words) = ""Kharab"" Then

    Words = ConvertDigit(Temp) &amp; place(iCount)
    MyNumber = Left(MyNumber, Len(MyNumber) - 1)

    Else

    Words = ConvertDigit(Temp) &amp; place(iCount) &amp; Words
    MyNumber = Left(MyNumber, Len(MyNumber) - 1)

    End If
    Else

    If Trim(Words) = ""Thousand"" Or _
    Trim(Words) = ""Lakh Thousand"" Or _
    Trim(Words) = ""Lakh"" Or _
    Trim(Words) = ""Crore"" Or _
    Trim(Words) = ""Crore Lakh Thousand"" Or _
    Trim(Words) = ""Arab Crore Lakh Thousand"" Or _
    Trim(Words) = ""Arab"" Then


    Words = ConvertTens(Temp) &amp; place(iCount)


    MyNumber = Left(MyNumber, Len(MyNumber) - 2)
    Else

    '=================================================================
    ' if only Lakh, Crore, Arab, Kharab

    If Trim(ConvertTens(Temp) &amp; place(iCount)) = ""Lakh"" Or _
    Trim(ConvertTens(Temp) &amp; place(iCount)) = ""Crore"" Or _
    Trim(ConvertTens(Temp) &amp; place(iCount)) = ""Arab"" Then

    Words = Words
    MyNumber = Left(MyNumber, Len(MyNumber) - 2)
    Else
    Words = ConvertTens(Temp) &amp; place(iCount) &amp; Words
    MyNumber = Left(MyNumber, Len(MyNumber) - 2)
    End If

    End If
    End If

    iCount = iCount + 2
    Loop

    RupeesToWord = Words &amp; Hundreds &amp; Paisa
    End Function

    ' Conversion for hundreds
    '*****************************************
    Private Function ConvertHundreds(ByVal MyNumber)
    Dim Result As String

    ' Exit if there is nothing to convert.
    If Val(MyNumber) = 0 Then Exit Function

    ' Append leading zeros to number.
    MyNumber = Right(""000"" &amp; MyNumber, 3)

    ' Do we have a hundreds place digit to convert?
    If Left(MyNumber, 1) &lt;&gt; ""0"" Then
    Result = ConvertDigit(Left(MyNumber, 1)) &amp; "" Hundred ""
    End If

    ' Do we have a tens place digit to convert?
    If Mid(MyNumber, 2, 1) &lt;&gt; ""0"" Then
    Result = Result &amp; ConvertTens(Mid(MyNumber, 2))
    Else
    ' If not, then convert the ones place digit.
    Result = Result &amp; ConvertDigit(Mid(MyNumber, 3))
    End If

    ConvertHundreds = Trim(Result)
    End Function

    ' Conversion for tens
    '*****************************************
    Private Function ConvertTens(ByVal MyTens)
    Dim Result As String

    ' Is value between 10 and 19?
    If Val(Left(MyTens, 1)) = 1 Then
    Select Case Val(MyTens)
    Case 10 : Result = ""Ten""
    Case 11 : Result = ""Eleven""
    Case 12 : Result = ""Twelve""
    Case 13 : Result = ""Thirteen""
    Case 14 : Result = ""Fourteen""
    Case 15 : Result = ""Fifteen""
    Case 16 : Result = ""Sixteen""
    Case 17 : Result = ""Seventeen""
    Case 18 : Result = ""Eighteen""
    Case 19 : Result = ""Nineteen""
    Case Else
    End Select
    Else
    ' .. otherwise it's between 20 and 99.
    Select Case Val(Left(MyTens, 1))
    Case 2 : Result = ""Twenty ""
    Case 3 : Result = ""Thirty ""
    Case 4 : Result = ""Forty ""
    Case 5 : Result = ""Fifty ""
    Case 6 : Result = ""Sixty ""
    Case 7 : Result = ""Seventy ""
    Case 8 : Result = ""Eighty ""
    Case 9 : Result = ""Ninety ""
    Case Else
    End Select

    ' Convert ones place digit.
    Result = Result &amp; ConvertDigit(Right(MyTens, 1))
    End If

    ConvertTens = Result
    End Function

    Private Function ConvertDigit(ByVal MyDigit)
    Select Case Val(MyDigit)
    Case 1 : ConvertDigit = ""One""
    Case 2 : ConvertDigit = ""Two""
    Case 3 : ConvertDigit = ""Three""
    Case 4 : ConvertDigit = ""Four""
    Case 5 : ConvertDigit = ""Five""
    Case 6 : ConvertDigit = ""Six""
    Case 7 : ConvertDigit = ""Seven""
    Case 8 : ConvertDigit = ""Eight""
    Case 9 : ConvertDigit = ""Nine""
    Case Else : ConvertDigit = """"
    End Select
    End Function



    Public Function GenerateVbCrLf(ByVal Count as integer)
    dim Value as String
    dim i as integer
    For i = 1 To Count Step 1
    Value = Value &amp; "" "" &amp; VbCrLf
    Next i
    Return Value
    End Function

Public Function PrintProduct(ProductName as String,ProductGroupCaption as String,ProductGroupName as String,SpecificationCaption as String,Specification as String,Dimension1Caption as String,Dimension1Name as String,Dimension2Caption as String,Dimension2Name as String,Dimension3Caption as String,Dimension3Name as String,Dimension4Caption as String,Dimension4Name as String,ProductUidCaption as String,ProductUidName as String)
dim str as String
Dim C As Integer = 0
  str =iif(ProductName  &lt;&gt; """",ProductName+""&lt;BR&gt;"" ,"""")
  if ProductGroupName &lt;&gt; """"  Then 
     str =str +""&lt;b&gt;""+ProductGroupCaption+""&lt;/b&gt;""+"" : ""+ ProductGroupName
  C=C+1
 End If


if Specification &lt;&gt; """"  Then
str =str +"" ""+iif(C mod 2=1,"" , "","""")    
  C=C+1
     str =str +""&lt;Br&gt;""+""&lt;b&gt;""+SpecificationCaption+""&lt;/b&gt;"" +""  : ""+ Specification+iif(C mod 2=0,""&lt;BR&gt;"","""")  
 End If

  if Dimension1Name &lt;&gt; """"  Then
    str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
     str =str +"" ""+""&lt;b&gt;""+Dimension1Caption+""&lt;/b&gt;"" + "" : ""+ Dimension1Name +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If      

  if Dimension2Name &lt;&gt; """"  Then
   str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
     str =str +""&lt;b&gt;""+Dimension2Caption+""&lt;/b&gt;"" +"" : ""+ Dimension2Name +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If   

  if Dimension3Name &lt;&gt; """"  Then
  str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
     str =str +""&lt;b&gt;""+Dimension3Caption+""&lt;/b&gt;"" +"" : ""+ Dimension3Name +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If   

  if Dimension4Name &lt;&gt; """"  Then
   str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
     str =str + ""&lt;b&gt;""+Dimension4Caption+""&lt;/b&gt;"" +"" : ""+ Dimension4Name +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If  

  if ProductUidName &lt;&gt; """"  Then
   str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
     str =str + ""&lt;b&gt;""+ProductUidCaption+""&lt;/b&gt;"" +"" : ""+ ProductUidName+iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If  
 
if Right(str, 4).toupper()=""&lt;BR&gt;"" Then
	str =str.Substring(0,str.Length-4)
End If
return str 
End Function

Public Function PrintLineRemark(Remark as String,Lot as String,DiscountPer as String,DiscountAmt as String,LossQty as String,RecQty as String)
dim str as String
Dim C As Integer = 0
  str =iif(Remark &lt;&gt; """",Remark ,"""")

  if Lot &lt;&gt; """"  Then 
     str =str +""&lt;BR&gt;""+""&lt;b&gt;""+""Lot""+""&lt;/b&gt;""+"" : ""+ Lot 
     C=C+1 
 End If

if LossQty&lt;&gt; """"  Then
    str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
       str =str +"" ""+""&lt;b&gt;""+""Loss/Short"" +""&lt;/b&gt;""+"" : ""+ LossQty+iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If 

if RecQty&lt;&gt; """"  Then
    str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
       str =str +"" ""+""&lt;b&gt;""+""Rec Qty"" +""&lt;/b&gt;""+"" : ""+ RecQty+iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If 

if DiscountPer &lt;&gt; """"  Then
    str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
       str =str +"" ""+""&lt;b&gt;""+""Dis Per"" +""&lt;/b&gt;""+"" : ""+ DiscountPer +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If  

if DiscountAmt &lt;&gt; """"  Then
    str =str +"" ""+iif(C mod 2=1,"" , "","""")    
     C=C+1
       str =str +"" ""+""&lt;b&gt;""+""Dis Amt"" +""&lt;/b&gt;""+"" : ""+ DiscountAmt +iif(C mod 2=0,""&lt;BR&gt;"","""")    
 End If

if Right(str, 4).toupper()=""&lt;BR&gt;"" Then
	str =str.Substring(0,str.Length-4)
End If

return str 
End Function
</Code>
  <EmbeddedImages>
    <EmbeddedImage Name=""SuryaLogo"">
      <MIMEType>image/jpeg</MIMEType>
      <ImageData>/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAkGBwgHBgkIBwgKCgkLDRYPDQwMDRsUFRAWIB0iIiAdHx8kKDQsJCYxJx8fLT0tMTU3Ojo6Iys/RD84QzQ5OjcBCgoKDQwNGg8PGjclHyU3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3Nzc3N//AABEIAGEAYAMBEQACEQEDEQH/xAAbAAACAwEBAQAAAAAAAAAAAAADBAABBQIGB//EADkQAAEDAwIDBgQDBgcAAAAAAAECAwQABRESITFBUQYTFGFxgRUikaEysfAjM0JSc4IkNDVjcsHx/8QAGQEAAwEBAQAAAAAAAAAAAAAAAAIDBAEF/8QAKhEAAgICAQQBAwQDAQAAAAAAAQIAAxESBBMhMUEiUXGBFDNhoSMywbH/2gAMAwEAAhEDEQA/APuNEJKISlKCeJArhOIScq7CIQU3AS5RmraVHKh3AQNwPP7VCsW7tvjHqVc16jTz7l3oR1WyQmXI8MwU4U9qxp96pZV1VKfWTW0VMHPqEtXc/Do4iv8AiGQgBDurVqHXNCV9NQn0gbBYdx7gpSLgq4xlRnW0xAD3yFD5j0xU3FpsBU/H3HQ16HI7+o/wFXk5SFpWkKQoKB5g5rgIPiE6rsJKISUQlZohErrbWbpGDEgrCAsL+RWDkVG6lbl1aUqtaptlilj7RQbw/JjQ+8Co2x1pwFDOMj6VoNZQCZKuQlpIX1FblZXXO0kW7m5lhhkAKZJwDjOwOcb53oNqIh2nG47PcHB8eo/fo8Cba3GLkoiM5jJScHIORipNetI3Y9pZqesNCJ3ZWoMO1NM29f8AhWwcKUd+OTnzzXF5CWjqA9p1aekOmB4mbZrGuLfpl1FyMhmTnS2NwMnO5zjbgKt1VdAFkEoau1nJ8w987QwbbNjW+Wl0rljGUDZIJ07+/SgVF1M7ZyVqcKfJmha7exbIojRQoNgk
/Mcnes9FK0povia7bWtbZvMcq0nJRCUaITPkwSu5MzvEvJDKCCyk/Kr2/XAVBqtrQ+3j1KC0LWUx59xPs12jj9oUSTHZda7hQB7zG4OcHb0O1anQpiY6OQt2cDxObk41ZVg26Cwl6UoqcUlvGs+eOJ3rzudzHp1CjJM2cfj1sSfENLgPXdiKt5RjKTkrbxnj+vvU7uM/LRC/x/iOlgqJx3jHweMqC1EdLi0NHKSVYP29at+jraoVtkgReqwYsJSrPGEFcNsuIbWrUSDk59/Sg8OsVGpSQDDrNtsYvFgv2iJJMYmStRBQgjGPv+sVKrjvxKm0+X0jPYLWGe05tLqLsouT4bJfjL+Ram90k9M7g7U/B5dlysGGCIl9KAg+Zx2m7St9n1xEORXX/EKI+Q40gY+p34V6KJsD3mO/kCkgYzmPxLcli4yZoddUqQBlCj8qcdKypTpYz5PebGtLIFx4mhV5OIXCZJjSIrbENb6Hl6XFpOzY23P3+lQttdGUKuc/1K1orAknGJSbzAVdFWwSUGYkZLW+evHhnG+K06HG3qZuqm+me8FeGXWYCxbGwhal5X3ScE54nbnwrDzjaav8XmaKBWH+UPa4zzURtMxZdeG+VblPlmn41TpWBYcmFjAsdR2j1aZOSiElEJWKIRG7RXZERaIjhaczq+XbV5E1l5VTvWRWcGUrYK2Wg7O0+YLYuCcuJUSjvBkpHL3rnCFopAt8wu0LfHxIb5bxePhPiB4zGdGD0zjPDON8Vt0bG3qZusm/Tz3hLbImvuyRMi9whDmGjqzrT1/XWs1L2MW3XH0mqxUUDQ5jxNXkpnIsluRdlXVLA8YoYLmo9MZxwzjbNMXOuskKUD9THeJ2mJNTc5D8xTmBkJyrZWTy8hXkcSm4XM9pP/JttdNAFE3civVmeXRCVmiEmaISZohJmiEw7rAluXViREUvBxqOrZGP+scq8vlce5uQrVmaK7FFZVo98It5uYuZjI8ZjHe8+GPT
ONs16uxxiY+km++O8eyKWUmP8Jkx7ZPYiT3vESNRbdcP7skbYqXHq6OcknJjchzauFGDiddmYU+32pDF1leJkBROvUVYHIZO5rS5DHImehHRMOcmJQo0S6SZyLitapyH1gN98pKmm8kIKACMApwcjiSa6SVAx4k1VXYhj3+8dfSWbtaWkuLUkNuglSslWEp3PU0vkGUIw6j7zUe/dL/4n8qWVPieXhNJmRezbUkrWhcIqWNahqIQjc4O/GqnttiZUGy15+kcbjNx7wq3MLdVEfirW8yp1Su6OQAQScp1Aq2z/DtS57Zj4xZoPBE6iXE26PIiXNxSnIaQpDhGVPtHZJ81Z+Ujr6igjPiCvoCrev7hGVu262yrlcNSpDiS6toHIR/K2n7DzJNcPc4E6CUQu3mB7Ph+DIdt01wrdWgSkqJ4lX7wD0Xv6LFdbuMzlOVJU/eH7T26XdLWqNBlmM6VpVqyRkDlkb0IwU5Ma9GsTVTgyvgpkW6BGmy3nHYukqdSrBcUBxNZ+RStxBPbBzNHGdqUx57YhHL5D+HzZjClPIhhRcSlJByOQzXabUuPwM5erUrsw/mV2cvKL7bBNQypkaykpUc8Oh5irumpxIU3C1NsYnU1i2XOCJbi21NJSVtykLwWx/MlY4VwEgwcI67TNZlOJb7PzrgrTqQpDjihgBS0jST0zj6mm+oEmCRozTbuUtmHAekPrCW0oJyee2wHUnpSAd5d2CrkzBbt6VJ7OwpqFZaiKSpIUU4UEIHEVTP+xEgE/bVvpG7a03Z7i5AKcNSlFyO6dyo/xIUo8SOIzy9KU/IZjIBW2p9wt5ZbcudlUtAURKVgkf7az+aQfYUKexnbQC6ff/hlXhCp86LbUOLbSn/EurSASAk/INwRurf+00L2GYWjdgv5i90ivQCxdVzZEgRF5cS4EAd0rAXwSOAwr+2gHPbEWxSmLM5xG+0l4+B2pU4MF/CgnSDgDPMnkK4i7NiNfb0k2AzKTf4y
LbCmyUOMiWE6UFOSkkc6hfclBAb2cTTx0a9cqPWZpNR2WQoNNIQFnUrSkDJ6mmVVXwMThJPmcNvRW3vCNuMpdCdQZSQCB1xT98ZiZUHUTztuats28SEybbFS4VFSP2YyVA/xDgT515/G572XNU34lbOJWqhwJ6d1lt5tTbraVtqGFIUMgj0r0MyZAIwYlGsdrjOpdYgMIWj8BCPwenT2ptjEFKDwI4phpTzbqkJLjeQhRG6c8cfSlzHx3zJIjMyAgPNpWELC06h+FQ4EedGYFQfMjjDTi21uNpUppWpskfhOCMj2JHvXcwIBkSw0h9b4bSHVgBS8bkDOB9zXIYGczGvdyU1NahtoS6lYw82oZ1BW2PpmvN5XMeq9UQfeaEpD1ktNNEmC0+i3h9lL4QNLGsatI8vavTwfMy7KDpmMKQheNSQrByMjgaUgHzHH8RO5G4h2N8PDRQXP2/eck+X3qNxtyvT/ADKV9PB3/ETHZuIO0RvfeO+Ix+DI0506c9eFauoddZj/AE69Xq+4xeXDCiLlRmUd9kAr07gda8/mOaazYg7zbUN21Y9oa1ThPiB3SUqB0qGNs+VPxb+vWGnLE0bEcrVJy6ISUQkohErrN8FDU8EFR4JGNs+flWfk39GstiPWuzYgLI87MiJfloT3oUQlenBI6/nUuE7XV72DvGuUI2FME52dgrvyL0e88SkcAr5ScYzj0r0NzrrMZ46Gzqe41bkXBEiUZzjSmSv9gEDcJ8/tWWoWhm3Pb1NdhrwNPzHsVeTicm5Ro8xiG44Uvv57saSfvUWvRXCHyY4qdkLDwJl9mLLcLUuabjcDMS8sFAJJxxyTngTngOlabGVgMCY+PVZWWLNnMZvjExxDJt6iC0okpSrHpXmc2q4qpp9TfS6ZO86Xcl2+LHNySS85kK7sZx+tq63KNFa9Ydz9ICrdjp4jSLnEXFTJ74JaUdIUoY36VdeVS1Ys27GIa2219yKuUQRlyUvBbSDhRQM4
PtQeVUENmewh02zjEWZuZuLElNvBS6gDSXQMHP8A5UE5f6lWFPkR2q6ZG8HZI8xgvruK93SMJUsHJ61zg03qGN3uFzocawHaqxS70mIIc9UTuHNSgM/Nw32PEcvWvTrcL5EwcilrcatjE0Y1yYdnu25KlmQwkFeU7Hhz9xWZb0aw1jyJtNLrWHPgx+rSclEINTLSnEuKbQVo/CojcehrhUE5InQSBgQcyWxCZL0p1LTYIGpVLZYta7McCdRGc4UZmL2f7PG2XOdcPHLkIlnUEkcATnJOd+lWNgdRiZKuOanYk+ZL5erazdolnmxnHVyCkpUBsgqOBzzx6VJ+Kt6fIZAjtyhVYE9mP3NiA1a1JllLEVoaioHGjzqL8Wu2vpY7TR1jWeoTKs7Nudtg8EtMiM7klZ31ct/pRXxEpQ14nOv1fmDELRercb7JskOK404zkqXj5VEYz58+dXTjLSmUGAZAcsW2ms+RO+0fZtN6lwpK5jjHhFasJGc7g5HQ7caotmgMW7j9Vgc4xNaBNjT2S7EdDiAopJHWoV2paNkOZretqzhhGA2gKKwkBR4nG5p8DOYk6rsJKISUQgZcViY0WpLSXWyQSlQyKR0VxhhkRlZlOVhEpCEBKQAAMADlTDsIvmZjKzKu7yJFtCRGA7mSsA5zx0nG3tUq7XNjIRgD3HepNVfOTD3lmG/bJDdyTmLoy5x4DflVTYKxufUmaur8D7g7AxBYtMdFrSRFwSjVnPHfOeea4louUWDwYCjof4/pOZi3ItxjmLbg6ZB0vPpABQBwyf1wqdtrqyqBkH+pSupCGYnB/wDZpAZG9ViQcaMxFR3cZlDSMk6UJwMmuKiqMKMRmZmOWMNTRZKISUQkohJRCSiE4HEV2cgbl/kZH9M/lUb/ANtvtKV/7iCsv+lxv6YpeL+yv2jX/uNG1cavIGdDhROiXROyUQkohP/Z</ImageData>
    </EmbeddedImage>
  </EmbeddedImages>
  <rd:ReportUnitType>Inch</rd:ReportUnitType>
  <rd:ReportID>f1b70379-da78-4ea9-bab5-cbb93d10df58</rd:ReportID>
</Report>";

            return StringCode;

        }
    }
}
