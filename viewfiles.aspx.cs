using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
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
	private ConfigManager _configManager = new ConfigManager();
	private string _sno;

	//关于提交的作业的信息
	public class UploadInfo
	{
		public CourseworkInfo Coursework;
		public bool IsSubmitted;
		public DateTime SubmittedTime;
		public string Rank;
	}


	public DataTable GetUploadData(string sno)
	{
		string[] fileNamesArray;
		string[] directoryNamesArray;

		DataTable courseworkDataTable = new DataTable();

		courseworkDataTable.Columns.Add("作业ID");
		courseworkDataTable.Columns.Add("作业名称");
		courseworkDataTable.Columns.Add("发布时间");
		courseworkDataTable.Columns.Add("最后期限");
		courseworkDataTable.Columns.Add("是否提交");
		courseworkDataTable.Columns.Add("提交时间");
		courseworkDataTable.Columns.Add("全班排名（按提交时间）");


		foreach (CourseworkInfo c in _configManager.CourseworkList)
		{
			DataRow row = courseworkDataTable.NewRow();

			row["作业名称"] = c.Name;
			courseworkDataTable.Rows.Add(row);
		}


		DataTable uploadDataTable = new DataTable();

		uploadDataTable.Columns.Add("作业ID");
		uploadDataTable.Columns.Add("发布时间");
		uploadDataTable.Columns.Add("最后期限");
		uploadDataTable.Columns.Add("是否提交");
		uploadDataTable.Columns.Add("提交时间");
		uploadDataTable.Columns.Add("全班排名（按提交时间）");




		try
		{
			directoryNamesArray = Directory.GetDirectories(Server.MapPath(_configManager.UploadDirectory));
			


			for (int i = directoryNamesArray.Length - 1; i >= 0; i--)
			{
				string tempDirPath = "";

				tempDirPath = directoryNamesArray[i];
				if (Directory.Exists(tempDirPath + "\\"))
				{
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


						if (_configManager.CourseworkList.Exists(c => c.ID == id))
						{
							CourseworkInfo courseworkInfo = _configManager.CourseworkList.Find(c => c.ID == id);

							DataRow row = uploadDataTable.NewRow();

							row[0] = id;
							row[1] = courseworkInfo.PublishTime.ToString("yyyy-MM-dd");
							row[2] = courseworkInfo.Deadline.ToString("yyyy-MM-dd");
							row[3] = "是";
							//row[4] = submittedTimeStr;
							

							int year = Convert.ToInt32(submittedTimeStr.Substring(0, 4));
							int month = Convert.ToInt32(submittedTimeStr.Substring(4, 2));
							int day = Convert.ToInt32(submittedTimeStr.Substring(6, 2));
							int hour = Convert.ToInt32(submittedTimeStr.Substring(8, 2));
							int minute = Convert.ToInt32(submittedTimeStr.Substring(10, 2));
							int second = Convert.ToInt32(submittedTimeStr.Substring(12, 2));


							DateTime d = new DateTime(year,month,day,hour,minute,second);

							row[4] = d.ToString("yyyy-MM-dd HH:mm:ss");

							uploadDataTable.Rows.Add(row);
						}

						
						//UploadInfo uploadInfo = new UploadInfo()
						//{
						//CourseworkInfo coursework = courseworkList.Find(c => c.ID == id);

						//uploadList.Add(new UploadInfo()
						//{
						//	Coursework = coursework,

						//});

						//}


						//string[] currentRow = new string[3];
						//currentRow[0] = Convert.ToString(filesDataTable.Rows.Count + 1);
						//currentRow[1] = fileNamesArray[j].Substring(currentFileName.LastIndexOf("\\") + 1);
						//currentRow[2] = directoryNamesArray[i].Substring(directoryNamesArray[i].LastIndexOf("\\") + 1);
						//filesDataTable.Rows.Add(currentRow);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Response.Write(ex.ToString());
		}

		return uploadDataTable;
	}

	public void BindData()
	{
		this.Title = "浏览提交的所有文件";
		this.EnableViewState = true;
		this.MaintainScrollPositionOnPostBack = true;

		DataTable dataTable = GetUploadData(_sno);


		if (dataTable.Rows.Count == 0)
		{
			Response.Write("<font color=red>没有搜索到你提交的任何作业，若有误，请与任课老师联系。</font>");
		}
		else
		{
			gvFiles.DataSource = dataTable;
			gvFiles.DataBind();
		}
	}


	public bool SnoIsValid(string sno)
	{
		if (!_configManager.StudentList.Exists(s => s.Sno == sno))
		{
			Response.Write("该学号不合法");
			return false;
		}
		return true;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		//string sno = Request.QueryString["sid"];
		string sno = "124173105";
		if (!Page.IsPostBack)
		{
			_configManager.LoadConfig(Server.MapPath("config.txt"));
			if (SnoIsValid(sno))
			{
				BindData();
			}
		}
	}
}