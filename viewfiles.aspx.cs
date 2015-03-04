using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.MobileControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

public partial class viewfiles : System.Web.UI.Page
{
	//关于提交的作业的信息
	public class UploadInfo
	{
		public CourseworkInfo Coursework;
		public bool IsSubmitted;
		public DateTime SubmittedTime;
		public string Rank;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		string currentUploadDir = "";
		string snoAndSname = "";
		string[] fileNamesArray;
		string[] directoryNamesArray;
		List<CourseworkInfo> courseworkList;

		if (Cache["courseworklist"] != null)
		{
			courseworkList = Cache["courseworklist"] as List<CourseworkInfo>;
		}

		this.Title = "浏览提交的所有文件";
		this.EnableViewState = true;
		this.MaintainScrollPositionOnPostBack = true;

		if (Session["uploaddirectory"] != null && Session["personalcategoryname"] != null)
		{
			currentUploadDir = Session["uploaddirectory"].ToString();
			snoAndSname = Session["sosname"].ToString();
		}
		else
		{
			Response.Redirect("upload.aspx");
		}


		if (Page.IsPostBack != true)
		{
			try
			{


				DataTable filesDataTable = new DataTable("filesDataTable");

				//filesDataTable.Columns.Add("SN");
				//filesDataTable.Columns.Add("提交的文件名");
				filesDataTable.Columns.Add("作业名称");
				filesDataTable.Columns.Add("发布时间");
				filesDataTable.Columns.Add("最后期限");
				filesDataTable.Columns.Add("是否提交");
				filesDataTable.Columns.Add("提交时间");
				filesDataTable.Columns.Add("全班排名（按提交时间）");



				
				directoryNamesArray = Directory.GetDirectories(currentUploadDir);
				for (int i = directoryNamesArray.Length - 1; i >= 0; i--)
				{
					string tempDirPath = "";

					tempDirPath = directoryNamesArray[i];
					//if (Directory.Exists(tempDirPath + "\\" + personalCatetoryName) == true)
					if (Directory.Exists(tempDirPath + "\\"))
					{
						//tempDirPath = tempDirPath + "\\" + personalCatetoryName;
						tempDirPath = tempDirPath + "\\";
						fileNamesArray = Directory.GetFiles(tempDirPath);

						for (int j = fileNamesArray.Length - 1; j >= 0; j--)
						{
							string currentFileName = fileNamesArray[j].Substring(fileNamesArray[j].LastIndexOf("\\") + 1);
							string[] paras = currentFileName.Split('+');

							string id = paras[0];
							string courseworkName = paras[1];
							string submittedTimeStr = paras[2];
							string snoAndSnameStr = paras[3];

							string[] currentRow = new string[3];
							currentRow[0] = Convert.ToString(filesDataTable.Rows.Count + 1);
							//currentRow[1] = fileNamesArray[j].Substring(currentFileName.LastIndexOf("\\") + 1);
							//currentRow[2] = directoryNamesArray[i].Substring(directoryNamesArray[i].LastIndexOf("\\") + 1);
							filesDataTable.Rows.Add(currentRow);
						}
					}

				}


				gvFiles.DataSource = filesDataTable;
				gvFiles.DataBind();

				if (filesDataTable.Rows.Count == 0)
				{
					Response.Write("<font color=red>没有搜索到你提交的任何作业，若有误，请与任课老师联系。</font>");
				}



			}

			catch (Exception ex)
			{
				Response.Write(ex.ToString());
			}
		}
	}


}
