#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;

#endregion

public partial class _Default : Page
{
	private static ConfigManager _configManager = new ConfigManager();

	protected void Page_Load(object sender, EventArgs e)
	{
		MaintainScrollPositionOnPostBack = true;
		EnableViewState = true;


		if (!Page.IsPostBack)
		{
			BindConfig();
			SetUploadEnabled();
		}
	}

	private void BindConfig()
	{
		_configManager.LoadConfig(Server.MapPath("config.txt"));
		lblTitle.Text = _configManager.PageTitle;
		lblNotice.Text = _configManager.NoticeInfo;

		dplCourseworkName.Items.Clear();
		foreach (CourseworkInfo c in _configManager.CourseworkList)
		{
			dplCourseworkName.Items.Add(string.Format("【{0}】 {1} (发布时间:{2}, 期限(天):{3})", c.ID,
			c.Name, c.PublishTime.ToString("yyyy-MM-dd"), c.DaysBeforeDeadline));
		}

	}

	protected void btnSubmit_Click(object sender, EventArgs e)
	{
		if (ValidateInput("upload"))
		{
			UploadFile();
		}
	}

	private void UploadFile()
	{
		string studentName = txtStudentName.Text.Trim();
		string studentID = txtStudentID.Text.Trim();
		string destFilePath = "";
		string destDirPath = "";

		string path = Server.MapPath("Images\\");
		string filename = "";
		int slashPos = -1;
		int dotPos = -1;
		string shortName = "";
		string fileType = "";

		filename = fupFile.PostedFile.FileName;
		slashPos = filename.LastIndexOf("\\");
		dotPos = filename.LastIndexOf(".");
		shortName = filename.Substring(slashPos + 1);
		fileType = filename.Substring(dotPos + 1);

		StreamWriter sw = null;

		try
		{
			string currentID = GetCourseworkID(dplCourseworkName.Text);

			CourseworkInfo currentCoursework = _configManager.CourseworkList.Find(c => c.ID == currentID);


			destFilePath = currentCoursework.ID + "+" + currentCoursework.Name + "+" + DateTime.Now.ToString("yyyyMMddHHmmss") +
						   "+" + studentID + studentName +
						   "." + fileType;
			destDirPath = Server.MapPath("") + "\\" + _configManager.UploadDirectory + currentCoursework.ID + "+" + currentCoursework.Name + "\\";


			if (Directory.Exists(destDirPath) == false)
			{
				Directory.CreateDirectory(destDirPath);
			}


			fupFile.SaveAs(destDirPath + destFilePath);

			sw = new StreamWriter(Server.MapPath("uploadhistory.txt"), true, Encoding.Default);
			sw.WriteLine(destFilePath + "," + DateTime.Now + "," +
						 Request.ServerVariables.Get("Remote_Addr"));
			sw.Close();

			lblNotice.Text = "上传文件成功，文件名为\"" + destFilePath + "\"。若想进一步确认，请点击\"查看我已经提交的所有文件\"进行查看。";
		}
		catch (Exception ex)
		{
			lblNotice.Text = ex.Message;
		}
		finally
		{
			if (sw != null)
			{
				sw.Close();
			}
		}
	}


	protected void btnReset_Click(object sender, EventArgs e)
	{
		Response.Redirect("upload.aspx");
	}

	private bool AutoGetStudentName()
	{
		string studentID = txtStudentID.Text.Trim();
		string[] studentRecordArray;
		bool foundStudentID = false;

		for (int i = 0; i < _configManager.StudentList.Count; i++)
		{
			//if (_configManager.StudentList[i].Sno == studentID)
			//{
			//	txtStudentName.Text = _configManager.StudentList[i].Name;
			//	foundStudentID = true;
			//	break;
			//}



		}

		StudentInfo student = _configManager.StudentList.Find(s => s.Sno == txtStudentID.Text.Trim());

		if (student != null)
		{
			txtStudentName.Text = student.Name;
		}

		else
		{
			lblNotice.Text = "此学号未经授权，请检查是否输入了错误的学号，若确认无误，请与任课任课老师联系。";
			txtStudentName.Text = "";
			btnSubmit.Enabled = false;
			lkbViewFiles.Enabled = false;
			return false;
		}
		lblNotice.Text = "注意！请在提交作业前仔细核对学号，姓名与所属的作业名称，错误的输入会导致文件提交到错误的位置。";

		btnSubmit.Enabled = true;
		lkbViewFiles.Enabled = true;
		return true;
	}

	protected void txtStudentID_TextChanged(object sender, EventArgs e)
	{
		BindConfig();
		AutoGetStudentName();
		SetUploadEnabled();
	}

	protected void lkbViewFiles_Click(object sender, EventArgs e)
	{
		string target = String.Format("viewfiles.aspx?sid={0}", txtStudentID.Text.Trim());
		Response.Redirect(target);

		//Response.Redirect("sina.aspx");
		//if (ValidateInput("browsefile"))
		//{
		//	Session["snosname"] = txtStudentID.Text.Trim() + "+" + txtStudentName.Text.Trim();
		//	Session["uploaddirectory"] = Server.MapPath("") + _configManager.UploadDirectory;
		//	//Cache["courseworklist"] = _configManager.CourseworkList;
		//	Response.Redirect(string.Format("viewfiles.aspx?sid={0}", txtStudentID.Text.Trim()));
		//}
	}

	private bool ValidateInput(string type)
	{
		//string studentIDPattern = @"^\d{9}";
		//string filePathPattern = @"^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w ]*.*))+\.(rar|RAR)$";
		bool ifValid = false;

		try
		{
			if (type == "upload")
			{
				if (txtStudentID.Text.Trim() == "")
				{
					lblNotice.Text = "请输入学号";
				}
				else if (fupFile.FileName.Trim() == "")
				{
					lblNotice.Text = "请选择要上传的文件";
				}
				else if (AutoGetStudentName() == false)
				{
					lblNotice.Text = "此学号未经授权，请检查是否输入了错误的学号，若确认无误，请与任课任课老师联系。";
				}
				//else if (Regex.IsMatch(fupFile.PostedFile.FileName, filePathPattern) == false)
				//{
				//    //lblNotice.Text = fupFile.PostedFile.FileName;
				//    lblNotice.Text = "无效的文件路径或文件名，请仔细检查。注意！只能提交RAR格式的文件。";
				//}
				else
				{
					lblNotice.Text = "注意！请在提交作业前仔细核对学号，姓名与所属的作业名称，错误的输入会导致文件提交到错误的位置。";

					ifValid = true;
				}
			}
			else if (type == "browsefile")
			{
				if (txtStudentID.Text.Trim() == "")
				{
					lblNotice.Text = "请输入学号";
				}
				else if (AutoGetStudentName() == false)
				{
					lblNotice.Text = "此学号未经授权，请检查是否输入了错误的学号，若确认无误，请与任课任课老师联系。";
				}
				else
				{
					ifValid = true;
				}
			}

			return ifValid;
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
	}

	protected void dplCourseworkName_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetUploadEnabled();
	}

	/// <summary>
	/// 设定是否允许提交
	/// </summary>
	private void SetUploadEnabled()
	{
		var id = GetCourseworkID(dplCourseworkName.Text);

		//lblUploadInfo.Text = id;

		CourseworkInfo coursework = _configManager.CourseworkList.Find(c => c.ID == id);
		DateTime deadline = coursework.PublishTime.AddDays(coursework.DaysBeforeDeadline);

		if (DateTime.Now > deadline.AddDays(1))
		{
			fupFile.Enabled = false;
			btnSubmit.Enabled = false;
			lblUploadInfo.Text = string.Format("该作业最后期限为{0}，已不允许提交。", deadline.ToString("yyyy-MM-dd"));
		}
		else
		{
			fupFile.Enabled = true;
			btnSubmit.Enabled = true;
			lblUploadInfo.Text = string.Format("该作业最后期限为{0}", deadline.ToString("yyyy-MM-dd"));
		}
	}

	/// <summary>
	/// 根据下拉菜单中的字符串获取作业ID
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	private static string GetCourseworkID(string s)
	{
		int startPos = s.IndexOf("【");
		int endPos = s.IndexOf("】");

		return s.Substring(startPos + 1, endPos - startPos - 1);
	}
}