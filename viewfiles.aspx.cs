#region

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;

#endregion

public partial class viewfiles : Page
{
	private static string _sno = "";
	private ConfigManager _configManager = new ConfigManager();

	//关于提交的作业的信息
	public DataTable GetUploadData(string sno)
	{
		var courseworkDataTable = GetCourseworkDataTable();
		var uploadFileDataTable = GetUploadFileDataTable();

		var resultTable = from coursework in courseworkDataTable.AsEnumerable()
						  let deadline = Convert.ToDateTime(coursework.Field<string>("最后期限"))
						  join uploadfile in uploadFileDataTable.AsEnumerable()
							  on coursework.Field<string>("作业ID") equals uploadfile.Field<string>("作业ID")
							  into resulttable
						  from uploadfile in resulttable.DefaultIfEmpty()
						  orderby uploadfile.Field<string>("提交时间") ascending
						  select new
						  {
							  Sno = uploadfile.Field<string>("学号"),
							  Sname = uploadfile.Field<string>("姓名"),
							  CourseworkName = coursework.Field<string>("作业名称"),
							  PublishTime = Convert.ToDateTime(coursework.Field<string>("发布时间")).ToString("yyyy-MM-dd"),
							  Deadline = deadline.ToString("yyyy-MM-dd"), //最后期限
							  SubmitTime =
								  (uploadfile != null) ? Convert.ToDateTime(uploadfile.Field<string>("提交时间")).ToString("yyyy-MM-dd HH:mm:ss") : "未提交",
							  IsOverdued = (DateTime.Now >= deadline.AddDays(1)) ? "已截止" : "未截止",
							  Rank = "N/A"
						  };


		var resultDataTable = new DataTable();
		resultDataTable.Columns.Add("学号");
		resultDataTable.Columns.Add("姓名");
		resultDataTable.Columns.Add("作业名称");
		resultDataTable.Columns.Add("发布时间");
		resultDataTable.Columns.Add("最后期限");
		resultDataTable.Columns.Add("是否截止");
		resultDataTable.Columns.Add("提交时间");
		resultDataTable.Columns.Add("该文件提交排名");

		foreach (var line in resultTable)
		{
			DataRow row = resultDataTable.NewRow();

			row["学号"] = line.Sno;
			row["姓名"] = line.Sname;
			row["作业名称"] = line.CourseworkName;
			row["发布时间"] = line.PublishTime;
			row["最后期限"] = line.Deadline;
			row["是否截止"] = line.IsOverdued;
			row["提交时间"] = line.SubmitTime;
			row["该文件提交排名"] = line.Rank;
			resultDataTable.Rows.Add(row);
		}

		return resultDataTable;
	}


	private DataTable GetUploadFileDataTable()
	{
		//该学生已提交的作业表
		var uploadFileDataTable = new DataTable();

		uploadFileDataTable.Columns.Add("学号");
		uploadFileDataTable.Columns.Add("姓名");
		uploadFileDataTable.Columns.Add("作业ID");
		uploadFileDataTable.Columns.Add("提交时间");

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
							fileNamesArray[j].Substring(fileNamesArray[j].LastIndexOf("\\", StringComparison.Ordinal) + 1);
						string[] paras = currentFileName.Split('+');

						string id = paras[0];
						string submittedTimeStr = paras[2];
						//得到学号
						string sid = paras[3].Substring(0, 9);
						string sname = paras[3].Substring(9, paras[3].LastIndexOf(".") - 9);

						//指定学号与作业的提交文件是否存在
						if (_configManager.CourseworkList.Exists(c => c.ID == id))
						{
							DataRow row = uploadFileDataTable.NewRow();
							CourseworkInfo courseworkInfo = _configManager.CourseworkList.Find(c => c.ID == id);

							int year = Convert.ToInt32(submittedTimeStr.Substring(0, 4));
							int month = Convert.ToInt32(submittedTimeStr.Substring(4, 2));
							int day = Convert.ToInt32(submittedTimeStr.Substring(6, 2));
							int hour = Convert.ToInt32(submittedTimeStr.Substring(8, 2));
							int minute = Convert.ToInt32(submittedTimeStr.Substring(10, 2));
							int second = Convert.ToInt32(submittedTimeStr.Substring(12, 2));

							var d = new DateTime(year, month, day, hour, minute, second);

							row["学号"] = sid;
							row["姓名"] = sname;
							row["作业ID"] = id;
							row["提交时间"] = d.ToString("yyyy-MM-dd HH:mm:ss");

							uploadFileDataTable.Rows.Add(row);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}


		return uploadFileDataTable;
	}


	private DataTable GetCourseworkDataTable()
	{
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

		return courseworkDataTable;
	}

	public void BindData()
	{
		Title = "浏览提交的所有文件";
		EnableViewState = true;
		MaintainScrollPositionOnPostBack = true;

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
			gvFiles.DataBound += gvFiles_DataBound;

			_sno = Request.QueryString["sid"];
			_configManager.LoadConfig(Server.MapPath("config.txt"));
			if (SnoIsValid(_sno))
			{
				BindData();
			}
		}
	}

	private void gvFiles_DataBound(object sender, EventArgs e)
	{
		int rowCount = gvFiles.Rows.Count;
		for (int i = 0; i < rowCount; i++)
		{
			gvFiles.Rows[i].Cells[0].Text = (i + 1).ToString();
		}
	}
}