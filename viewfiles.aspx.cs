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
	private static string _sno = "";

	//关于提交的作业的信息
	public DataTable GetUploadData(string sno)
	{
		//目前为止应提交的所有作业列表
		var courseworkDataTable = new DataTable();

		courseworkDataTable.Columns.Add("作业ID");
		courseworkDataTable.Columns.Add("作业名称");
		courseworkDataTable.Columns.Add("发布时间");
		courseworkDataTable.Columns.Add("最后期限");

		foreach (CourseworkInfo c in _configManager.CourseworkList)
		{
			DataRow row = courseworkDataTable.NewRow();

			row["作业ID"] = c.ID;
			row["作业名称"] = c.Name;
			row["发布时间"] = c.PublishTime;
			row["最后期限"] = c.Deadline;
			courseworkDataTable.Rows.Add(row);
		}

		//该学生已提交的作业表
		var uploadDataTable = new DataTable();
		uploadDataTable.Columns.Add("作业ID");
		uploadDataTable.Columns.Add("提交时间");

		var resultDataTable = new DataTable();

		//resultDataTable.Columns.Add("作业ID");
		resultDataTable.Columns.Add("作业名称");
		resultDataTable.Columns.Add("发布时间");
		resultDataTable.Columns.Add("最后期限");
		resultDataTable.Columns.Add("是否截止");
		resultDataTable.Columns.Add("提交时间");
		resultDataTable.Columns.Add("全班排名（按提交时间）");


		try
		{
			string[] directoryNamesArray = Directory.GetDirectories(Server.MapPath(_configManager.UploadDirectory));


			for (int i = directoryNamesArray.Length - 1; i >= 0; i--)
			{
				string tempDirPath = directoryNamesArray[i] + "\\";

				//若作业文件夹存在
				if (Directory.Exists(tempDirPath))
				{
					//得到该文件夹下所有文件
					string[] fileNamesArray = Directory.GetFiles(tempDirPath);

					for (int j = fileNamesArray.Length - 1; j >= 0; j--)
					{
						string currentFileName =
							fileNamesArray[j].Substring(fileNamesArray[j].LastIndexOf("\\", System.StringComparison.Ordinal) + 1);
						string[] paras = currentFileName.Split('+');

						string id = paras[0];
						string submittedTimeStr = paras[2];
						//得到学号
						string sid = paras[3].Substring(0, 9);

						//指定学号与作业的提交文件是否存在
						if (_configManager.CourseworkList.Exists(c => c.ID == id) && sid == sno)
						{
							DataRow row = uploadDataTable.NewRow();
							CourseworkInfo courseworkInfo = _configManager.CourseworkList.Find(c => c.ID == id);

							int year = Convert.ToInt32(submittedTimeStr.Substring(0, 4));
							int month = Convert.ToInt32(submittedTimeStr.Substring(4, 2));
							int day = Convert.ToInt32(submittedTimeStr.Substring(6, 2));
							int hour = Convert.ToInt32(submittedTimeStr.Substring(8, 2));
							int minute = Convert.ToInt32(submittedTimeStr.Substring(10, 2));
							int second = Convert.ToInt32(submittedTimeStr.Substring(12, 2));

							var d = new DateTime(year, month, day, hour, minute, second);

							row["作业ID"] = id;
							row["提交时间"] = d.ToString("yyyy-MM-dd HH:mm:ss");

							uploadDataTable.Rows.Add(row);
						}
					}
				}
			}

			var resultTable = from coursework in courseworkDataTable.AsEnumerable()
							  let deadline = Convert.ToDateTime(coursework.Field<string>("最后期限"))
							  join upload in uploadDataTable.AsEnumerable()
								  on coursework.Field<string>("作业ID") equals upload.Field<string>("作业ID")
								  into nullTable
							  from upload in nullTable.DefaultIfEmpty()
							  select new
							  {
								  //C0 = coursework.Field<string>("作业ID"),
								  CourseworkName = coursework.Field<string>("作业名称"),
								  PublishTime = Convert.ToDateTime(coursework.Field<string>("发布时间")).ToString("yyyy-MM-dd"),
								  Deadline = deadline.ToString("yyyy-MM-dd"),
								  SubmitTime = (upload != null) ? Convert.ToDateTime(upload.Field<string>("提交时间")).ToString("yyyy-MM-dd HH:mm:ss") : "未提交",
								  IsOverdued = (DateTime.Now >= deadline.AddDays(1)) ? "已截止" : "未截止",
								  Rank = 1
							  };

			foreach (var line in resultTable)
			{
				DataRow row = resultDataTable.NewRow();

				//row[0] = line.C0;
				row["作业名称"] = line.CourseworkName;
				row["发布时间"] = line.PublishTime;
				row["最后期限"] = line.Deadline;
				row["是否截止"] = line.IsOverdued;
				row["提交时间"] = line.SubmitTime;
				row["全班排名（按提交时间）"] = line.Rank;
				resultDataTable.Rows.Add(row);
			}
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}

		return resultDataTable;
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


		if (!Page.IsPostBack)
		{
			_sno = Request.QueryString["sid"];
			_configManager.LoadConfig(Server.MapPath("config.txt"));
			if (SnoIsValid(_sno))
			{
				BindData();
			}
		}
	}
}