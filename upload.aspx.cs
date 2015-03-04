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
	private static string pageTitle = "";
	private static string uploadDirectoryStr = "";
	private static List<CourseworkInfo> courseworkList = new List<CourseworkInfo>();
	private static List<StudentInfo> studentList = new List<StudentInfo>();

	protected void Page_Load(object sender, EventArgs e)
	{
		MaintainScrollPositionOnPostBack = true;
		EnableViewState = true;
		Title = pageTitle;


		if (Page.IsPostBack != true)
		{
			LoadConfig();
		}
		SetUploadEnabled();
	}

	private void LoadConfig()
	{
		courseworkList.Clear();
		studentList.Clear();

		StreamReader sr = null;

		try
		{
			sr = new StreamReader(Server.MapPath("config.txt"), Encoding.Default);

			string currentLine = "";

			dplCourseworkName.Items.Clear();

			while ((currentLine = sr.ReadLine()) != null)
			{
				if (currentLine == "[Title]")
				{
					lblTitle.Text = sr.ReadLine().Trim();
					pageTitle = lblTitle.Text;
				}
				else if (currentLine == "[Notice Info]")
				{
					lblNotice.Text = sr.ReadLine().Trim();
				}
				else if (currentLine == "[Upload Directory]")
				{
					uploadDirectoryStr = sr.ReadLine().Trim();
				}
				else if (currentLine == "[Student List]")
				{
					string studentRecordStr = "";

					while (studentRecordStr != "[Coursework List]")
					{
						studentRecordStr = sr.ReadLine().Trim();

						if (studentRecordStr != "[Coursework List]")
						{
							studentList.Add(new StudentInfo { Sno = studentRecordStr.Split(',')[0], Name = studentRecordStr.Split(',')[1] });
						}
					}

					string[] a = sr.ReadLine().Trim().Split(',');

					CourseworkInfo latestCoursework = new CourseworkInfo
					{
						ID = a[0],
						Name = a[1],
						PublishTime = Convert.ToDateTime(a[2]),
						DaysBeforeDeadline = Convert.ToInt32(a[3])
					};
					courseworkList.Add(latestCoursework);

					dplCourseworkName.Items.Add(string.Format("【{0}】 {1} (发布时间:{2}, 期限(天):{3})", latestCoursework.ID,
						latestCoursework.Name, latestCoursework.PublishTime.ToString("yyyy-MM-dd"), latestCoursework.DaysBeforeDeadline));
				}
				else
				{
					string[] a = currentLine.Split(',');

					CourseworkInfo latestCoursework = new CourseworkInfo
					{
						ID = a[0],
						Name = a[1],
						PublishTime = Convert.ToDateTime(a[2]),
						DaysBeforeDeadline = Convert.ToInt32(a[3])
					};
					courseworkList.Add(latestCoursework);
					//dplCourseworkName.Items.Add(latestCoursework.Name);
					dplCourseworkName.Items.Add(string.Format("【{0}】 {1} (发布时间:{2}, 期限(天):{3})", latestCoursework.ID,
						latestCoursework.Name, latestCoursework.PublishTime.ToString("yyyy-MM-dd"), latestCoursework.DaysBeforeDeadline));
				}
			}
		}
		catch (Exception ex)
		{
			lblNotice.Text = "加载本页面出错，请稍候再试。具体错误如下：<br>" + ex.Message;
		}
		finally
		{
			if (sr != null)
			{
				sr.Close();
			}
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

			CourseworkInfo currentCoursework = courseworkList.Find(c => c.ID == currentID);



			destFilePath = currentCoursework.ID + "+" + currentCoursework.Name + "+" + DateTime.Now.ToString("yyyyMMddHHmmss") + "+" + studentID + studentName +
						   "." + fileType;
			destDirPath = Server.MapPath("") + uploadDirectoryStr + currentCoursework.ID + "+" + currentCoursework.Name + "\\";



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
			//lblNotice.Text = "上传文件失败，请稍候重试。";
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

		for (int i = 0; i < studentList.Count; i++)
		{
			//studentRecordArray = (string[])studentList[i];
			//if (studentRecordArray[0] == studentID)
			if (studentList[i].Sno == studentID)
			{
				//txtStudentName.Text = studentRecordArray[1];
				txtStudentName.Text = studentList[i].Name;
				foundStudentID = true;
				break;
			}
		}

		if (foundStudentID == false)
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
		LoadConfig();
		AutoGetStudentName();
		SetUploadEnabled();
	}

	protected void lkbViewFiles_Click(object sender, EventArgs e)
	{
		if (ValidateInput("browsefile"))
		{
			Session["snosname"] = txtStudentID.Text.Trim() + "+" + txtStudentName.Text.Trim();
			Session["uploaddirectory"] = Server.MapPath("") + uploadDirectoryStr;
			Cache["courseworklist"] = courseworkList;
			Response.Redirect("viewfiles.aspx");
		}
	}

	private bool ValidateInput(string type)
	{
		//string studentIDPattern = @"^\d{9}";
		string filePathPattern = @"^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w ]*.*))+\.(rar|RAR)$";
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

	//设定是否允许提交
	private void SetUploadEnabled()
	{
		var id = GetCourseworkID(dplCourseworkName.Text);

		//lblUploadInfo.Text = id;

		CourseworkInfo coursework = courseworkList.Where(c => c.ID == id).ToList()[0];

		DateTime deadline = coursework.PublishTime.AddDays(coursework.DaysBeforeDeadline);

		if (DateTime.Now > deadline.AddDays(1))
		{
			fupFile.Enabled = false;
			lblUploadInfo.Text = string.Format("该作业最后期限为{0}，已不允许提交。", deadline.ToString("yyyy-MM-dd"));
		}
		else
		{
			fupFile.Enabled = true;
			lblUploadInfo.Text = string.Format("该作业最后期限为{0}", deadline.ToString("yyyy-MM-dd"));
		}
	}

	private static string GetCourseworkID(string s)
	{
		int startPos = s.IndexOf("【");
		int endPos = s.IndexOf("】");

		return s.Substring(startPos + 1, endPos - startPos - 1);
	}
}