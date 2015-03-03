using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

public partial class viewfiles : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		string currentUploadDir = "";
		string personalCatetoryName = "";
		string[] fileNamesArray;
		string[] directoryNamesArray;

		this.Title = "浏览提交的所有文件";
		this.EnableViewState = true;
		this.MaintainScrollPositionOnPostBack = true;

		if (Session["uploaddirectory"] != null && Session["personalcategoryname"] != null)
		{
			currentUploadDir = Session["uploaddirectory"].ToString();
			personalCatetoryName = Session["personalcategoryname"].ToString();
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

				filesDataTable.Columns.Add("SN");
				filesDataTable.Columns.Add("提交的文件名");
				filesDataTable.Columns.Add("该文件所属的作业名称");

								

				directoryNamesArray = Directory.GetDirectories(currentUploadDir);
				for (int i = directoryNamesArray.Length-1; i >=0; i--)
				{
					string tempDirPath = "";

					tempDirPath = directoryNamesArray[i];
					if (Directory.Exists(tempDirPath + "\\" + personalCatetoryName) == true)
					{
						tempDirPath = tempDirPath + "\\" + personalCatetoryName;
						fileNamesArray = Directory.GetFiles(tempDirPath);

						for (int j = fileNamesArray.Length-1; j >=0; j--)
						{
							string[] currentRow = new string[3];
							currentRow[0] = Convert.ToString( filesDataTable.Rows.Count + 1);
							currentRow[1] = fileNamesArray[j].Substring(fileNamesArray[j].LastIndexOf("\\") + 1);
							currentRow[2] = directoryNamesArray[i].Substring(directoryNamesArray[i].LastIndexOf("\\") + 1);
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
