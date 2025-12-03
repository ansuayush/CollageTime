using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Routing;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
using ExecViewHrk.EfAdmin;
using ExecViewHrk.WebUI.Helpers;

namespace ExecViewHrk.WebUI.Controllers
{
    public class FormController : Controller
    {
        // GET: Form
        public ActionResult FormTemplates()
        {
            return View();
        }


        public ActionResult FormTemplates_Read([DataSourceRequest]DataSourceRequest request)
        {
            try
            {

                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var formTemplates = clientDbContext.FormTemplates.OrderBy(e => e.Description).ToList();


                return Json(formTemplates.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }

            catch (Exception err)
            {
                return Json(new DataSourceResult
                {
                    Errors = err.Message
                });
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormTemplate_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.FormTemplate formTemplate)
        {

            if (formTemplate != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var formTemplateInDb = clientDbContext.FormTemplates
                    .Where(x => x.FormTemplateName == formTemplate.FormTemplateName)
                    .SingleOrDefault();

                if (formTemplateInDb != null)
                {
                    ModelState.AddModelError("", "The FormTemplate" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                }
                else
                {
                    var newFormTemplate = new FormTemplate { FormTemplateName = formTemplate.FormTemplateName, Description = formTemplate.Description };
                    clientDbContext.FormTemplates.Add(newFormTemplate);
                    clientDbContext.SaveChanges();
                    formTemplate.FormTemplateId = newFormTemplate.FormTemplateId;
                }

            }

            return Json(new[] { formTemplate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormTemplate_Update([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.FormTemplate formTemplate)
        {
            if (formTemplate != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var formTemplateInDb = clientDbContext.FormTemplates
                    .Where(x => x.FormTemplateId == formTemplate.FormTemplateId)
                    .SingleOrDefault();

                if (formTemplateInDb != null)
                {
                    formTemplateInDb.FormTemplateName = formTemplate.FormTemplateName;
                    formTemplateInDb.Description = formTemplate.Description;
                    clientDbContext.SaveChanges();
                }
            }

            return Json(new[] { formTemplate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormTemplate_Destroy([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.FormTemplate formTemplate)
        {
            if (formTemplate != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                FormTemplate formTemplateInDb = clientDbContext.FormTemplates.Where(x => x.FormTemplateId == formTemplate.FormTemplateId).SingleOrDefault();

                if (formTemplateInDb != null)
                {
                    clientDbContext.FormTemplates.Remove(formTemplateInDb);
                    clientDbContext.SaveChanges();
                }
            }

            return Json(new[] { formTemplate }.ToDataSourceResult(request, ModelState));
        }



        /////////////// FORM TEMPLATE FIELDS

        public ActionResult FormTemplateFields(string requestType )
        {
            // refactor this
            int nId = Convert.ToInt32(requestType);
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            PopulateFormTemplateFieldTypes();
            PopulateFormTemplateSelectionGroups();
            
            string formTemplateName = clientDbContext.FormTemplates
                .Where(x => x.FormTemplateId == nId).Select(x => x.FormTemplateName)
                .SingleOrDefault();
            ViewBag.FormTemplateName = formTemplateName;

            // refactor this
            List<FormTemplateFieldVm> formTemplateFieldVmList = clientDbContext.FormTemplateFields
                .Where(x => x.FormTemplateId == nId)
                .Select(x => new FormTemplateFieldVm
                {
                    ColumnNumber = x.ColumnNumber,
                    FormTemplateFieldId = x.FormTemplateFieldId,
                    FormTemplateId = x.FormTemplateId,
                    HtmlId = x.HtmlId,
                    Label = x.Label,
                    Position = x.Position,
                    Required = x.Required,
                    Type = x.Type,
                    Value = x.Value,
                    VisualWidth = x.VisualWidth == null ? 250 : (int)x.VisualWidth,
                    SelectionGroup = x.SelectionGroup == null ? "" : x.SelectionGroup,
                    CheckBoxRadioGroupName = x.CheckBoxRadioGroupName == null ? "" : x.CheckBoxRadioGroupName
                }).OrderBy(e => e.Position).ToList();


            return View(formTemplateFieldVmList);
        }

        [HttpPost]
        public ActionResult FormTemplateFields(string requestType, string submit, List<FormTemplateFieldVm> formTemplateVmList)
        {
            // refactor this
            int nId = Convert.ToInt32(requestType);
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            PopulateFormTemplateFieldTypes();
            PopulateFormTemplateSelectionGroups();

            string formTemplateName = clientDbContext.FormTemplates
                .Where(x => x.FormTemplateId == nId).Select(x => x.FormTemplateName)
                .SingleOrDefault();
            ViewBag.FormTemplateName = formTemplateName;

            
            // refactor this
            List<FormTemplateFieldVm> formTemplateFieldVmList = clientDbContext.FormTemplateFields
                .Where(x => x.FormTemplateId == nId)
                .Select(x => new FormTemplateFieldVm
                {
                    ColumnNumber = x.ColumnNumber,
                    FormTemplateFieldId = x.FormTemplateFieldId,
                    FormTemplateId = x.FormTemplateId,
                    HtmlId = x.HtmlId,
                    Label = x.Label,
                    Position = x.Position,
                    Required = x.Required,
                    Type = x.Type,
                    Value = x.Value,
                    VisualWidth = x.VisualWidth == null ? 250 : (int)x.VisualWidth,
                    SelectionGroup = x.SelectionGroup == null ? "" : x.SelectionGroup,
                    CheckBoxRadioGroupName = x.CheckBoxRadioGroupName == null ? "" : x.CheckBoxRadioGroupName
                }).OrderBy(e => e.Position).ToList();


            return View(formTemplateFieldVmList);
        }

        public ActionResult FormTemplateFields_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            try
            {

                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                // refactor this
                List<FormTemplateFieldVm> formTemplateFieldVmList = clientDbContext.FormTemplateFields
                    .Where(x => x.FormTemplateId == id)
                    .Select(x => new FormTemplateFieldVm { ColumnNumber = x.ColumnNumber,
                        FormTemplateFieldId = x.FormTemplateFieldId,
                        FormTemplateId = x.FormTemplateId,
                        HtmlId = x.HtmlId,
                        Label = x.Label,
                        Position = x.Position,
                        Required = x.Required,
                        Type = x.Type,
                        Value = x.Value,
                        VisualWidth = x.VisualWidth == null ? 250 : (int)x.VisualWidth,
                        SelectionGroup = x.SelectionGroup == null ? "" : x.SelectionGroup,
                        CheckBoxRadioGroupName = x.CheckBoxRadioGroupName == null ? "" : x.CheckBoxRadioGroupName
                    }).OrderBy(e => e.Position).ToList();

                


                return Json(formTemplateFieldVmList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }

            catch (Exception err)
            {
                return Json(new DataSourceResult
                {
                    Errors = err.Message
                });
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormTemplateField_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.WebUI.Models.FormTemplateFieldVm formTemplateField, int id)
        {

            if (formTemplateField != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var formTemplateFieldInDb = clientDbContext.FormTemplateFields
                    .Where(x => x.HtmlId == formTemplateField.HtmlId && x.FormTemplateId == id)
                    .SingleOrDefault();

                if (formTemplateFieldInDb != null)
                {
                    ModelState.AddModelError("", "The Form Template Field HtmlId is already defined.");
                }
                else
                {
                    var newFormTemplateField = new FormTemplateField
                        {
                            FormTemplateId = id,
                            HtmlId = formTemplateField.HtmlId,
                            Position = formTemplateField.Position,
                            ColumnNumber = formTemplateField.ColumnNumber,
                            Label = formTemplateField.Label,
                            Required = formTemplateField.Required,
                            Type = formTemplateField.Type,
                            Value = formTemplateField.Value,
                            VisualWidth = formTemplateField.VisualWidth,
                            SelectionGroup = formTemplateField.SelectionGroup,
                            CheckBoxRadioGroupName = formTemplateField.CheckBoxRadioGroupName
                        };

                    clientDbContext.FormTemplateFields.Add(newFormTemplateField);
                    clientDbContext.SaveChanges();
                    formTemplateField.FormTemplateFieldId = newFormTemplateField.FormTemplateFieldId;
                }

            }

            return Json(new[] { formTemplateField }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormTemplateField_Update([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.FormTemplateField formTemplateField)
        {
            if (formTemplateField != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var formTemplateFieldInDb = clientDbContext.FormTemplateFields
                    .Where(x => x.FormTemplateFieldId == formTemplateField.FormTemplateFieldId)
                    .SingleOrDefault();

                if (formTemplateFieldInDb != null)
                {
                    formTemplateFieldInDb.HtmlId = formTemplateField.HtmlId;
                    formTemplateFieldInDb.Position = formTemplateField.Position;
                    formTemplateFieldInDb. ColumnNumber = formTemplateField.ColumnNumber;
                    formTemplateFieldInDb.Label = formTemplateField.Label;
                    formTemplateFieldInDb.Required = formTemplateField.Required;
                    formTemplateFieldInDb.Type = formTemplateField.Type;
                    formTemplateFieldInDb.Value = formTemplateField.Value;
                    formTemplateFieldInDb.VisualWidth = formTemplateField.VisualWidth;
                    formTemplateFieldInDb.SelectionGroup = formTemplateField.SelectionGroup;
                    formTemplateFieldInDb.CheckBoxRadioGroupName = formTemplateField.CheckBoxRadioGroupName;
                    clientDbContext.SaveChanges();
                }
            }

            return Json(new[] { formTemplateField }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormTemplateField_Destroy([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.FormTemplateField formTemplateField)
        {
            if (formTemplateField != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                FormTemplateField formTemplateFieldInDb = clientDbContext.FormTemplateFields
                    .Where(x => x.FormTemplateFieldId == formTemplateField.FormTemplateFieldId).SingleOrDefault();

                if (formTemplateFieldInDb != null)
                {
                    clientDbContext.FormTemplateFields.Remove(formTemplateFieldInDb);
                    clientDbContext.SaveChanges();
                }
            }

            return Json(new[] { formTemplateField }.ToDataSourceResult(request, ModelState));
        }

        ///////////////////////   END FormTemplateFields /////////////////////////////////////////////////

        private void PopulateFormTemplateFieldTypes()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var fieldTypes = clientDbContext.FormTemplateFieldTypes.Select(x => new { x.Name }).OrderBy(x => x.Name).ToList();
                        //.Select(e => new MajorDdlEntryViewModel
                        //{
                        //    MajorId = e.MajorId,
                        //    MajorName = e.MajorName + "-" + e.MajorId.ToString()
                        //})
                        //.OrderBy(e => e.MajorName).ToList();

            //majorsList.Insert(0, new MajorDdlEntryViewModel { MajorId = 0, MajorName = "--select one--" });

            ViewData["FormTemplateFieldTypes"] = fieldTypes;
            ViewData["defaultFieldType"] = fieldTypes.First();
        }

        private void PopulateFormTemplateSelectionGroups()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var selectionGroups = clientDbContext.FormTemplateSelectionGroups.Select(x => new { x.Name }).OrderBy(x => x.Name).ToList();
            selectionGroups.Insert(0, new  { Name = "None" });

            ViewData["FormTemplateSelectionGroups"] = selectionGroups;
            //ViewData["defaultFieldType"] = fieldTypes.First();
        }

        public JsonResult FormTemplateFieldsSelectionGroup_Read(string selectionGroupName, int formId)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            FormTemplateSelectionGroup dropDownList = clientDbContext.FormTemplateSelectionGroups
                .Where(x => x.Name == selectionGroupName).SingleOrDefault();

            List<GenericLookupEntry> genericLookupList = GetLookupTable(dropDownList.ExecViewSql);
             

            return Json(genericLookupList, JsonRequestBehavior.AllowGet);

        }

        public class GenericLookupEntry
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }

        public List<GenericLookupEntry> GetLookupTable(string sql)
        {
            string connString = User.Identity.GetClientConnectionString();
            SqlConnection connection = new SqlConnection(connString);
        
            SqlDataReader reader = null;
            List<GenericLookupEntry> genericLookupList = new List<GenericLookupEntry>();
            try
            {
                SqlCommand cmdSelect = connection.CreateCommand();
                cmdSelect.CommandType = System.Data.CommandType.Text;
                cmdSelect.CommandText = sql;

                //cmdSelect.Parameters.Add(new SqlParameter("@EmployerJobId", SqlDbType.VarChar, 50));

                //cmdSelect.Parameters["@EmployerJobId"].Value = employerJobId;

                cmdSelect.Connection = connection;

                connection.Open();

                reader = cmdSelect.ExecuteReader();

                while (reader.Read())
                {

                    GenericLookupEntry genericLookupEntry = new GenericLookupEntry();

                    genericLookupEntry.Code = reader["Code"] == System.DBNull.Value ? "" : reader["Code"].ToString();
                    genericLookupEntry.Description = reader["Description"] == System.DBNull.Value ? "" : reader["Description"].ToString();
                    genericLookupList.Add(genericLookupEntry);
                }

            }
            catch// (Exception ex)
            {
                throw;
            }
            finally
            {
                // Close data reader object and database connection
                if (reader != null)
                    reader.Close();

                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }

            return genericLookupList;

        }


        ///////////////////////////////////////////////

        public ActionResult FormTemplateSelectionGroups()
        {

            return View();
        }


        public ActionResult FormTemplateSelectionGroups_Read([DataSourceRequest]DataSourceRequest request)
        {
            try
            {

                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var selectionGroups = clientDbContext.FormTemplateSelectionGroups.OrderBy(e => e.Description).ToList();


                return Json(selectionGroups.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }

            catch (Exception err)
            {
                return Json(new DataSourceResult
                {
                    Errors = err.Message
                });
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormTemplateSelectionGroup_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.FormTemplateSelectionGroup formTemplateSelectionGroup)
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            try
            {
                if (formTemplateSelectionGroup != null && ModelState.IsValid)
                {
                    
                    var formTemplateSelectionGroupInDb = clientDbContext.FormTemplateSelectionGroups
                        .Where(x => x.Name == formTemplateSelectionGroup.Name)
                        .SingleOrDefault();

                    if (formTemplateSelectionGroupInDb != null)
                    {
                        ModelState.AddModelError("", "The Form Template Selection Group" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                    }
                    else
                    {
                        var newFormTemplateSelectionGroup = new FormTemplateSelectionGroup
                        {
                            Name = formTemplateSelectionGroup.Name,
                            Description = formTemplateSelectionGroup.Description,
                            ExecViewTable = formTemplateSelectionGroup.ExecViewTable,
                            ExecViewTextColumn = formTemplateSelectionGroup.ExecViewTextColumn,
                            ExecViewValueColumn = formTemplateSelectionGroup.ExecViewValueColumn,
                            ExecViewSql = formTemplateSelectionGroup.ExecViewSql
                        };
                        clientDbContext.FormTemplateSelectionGroups.Add(newFormTemplateSelectionGroup);
                        clientDbContext.SaveChanges();
                        formTemplateSelectionGroup.FormTemplateSelectionGroupId = newFormTemplateSelectionGroup.FormTemplateSelectionGroupId;
                    }

                }

            }
            catch (Exception err)
            {
                IEnumerable<DbEntityValidationResult> errors = clientDbContext.GetValidationErrors();
                if (errors.Count() == 0)
                {

                }
                else
                {
                    foreach (DbEntityValidationResult error in errors)
                    {
                        foreach (var valError in error.ValidationErrors)
                        {
                            ModelState.AddModelError("", valError.ErrorMessage);
                        }
                    }
                }
                ModelState.AddModelError("", err.Message);
            }
            

            return Json(new[] { formTemplateSelectionGroup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormTemplateSelectionGroup_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.FormTemplateSelectionGroup formTemplateSelectionGroup)
        {
            if (formTemplateSelectionGroup != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var formTemplateSelectionGroupInDb = clientDbContext.FormTemplateSelectionGroups
                    .Where(x => x.FormTemplateSelectionGroupId == formTemplateSelectionGroup.FormTemplateSelectionGroupId)
                    .SingleOrDefault();

                if (formTemplateSelectionGroupInDb != null)
                {
                    formTemplateSelectionGroupInDb.Name = formTemplateSelectionGroup.Name;
                    formTemplateSelectionGroupInDb.Description = formTemplateSelectionGroup.Description;
                    formTemplateSelectionGroupInDb.ExecViewTable = formTemplateSelectionGroup.ExecViewTable;
                    formTemplateSelectionGroupInDb.ExecViewTextColumn = formTemplateSelectionGroup.ExecViewTextColumn;
                    formTemplateSelectionGroupInDb.ExecViewValueColumn = formTemplateSelectionGroup.ExecViewValueColumn;
                    formTemplateSelectionGroupInDb.ExecViewSql = formTemplateSelectionGroup.ExecViewSql;
                    clientDbContext.SaveChanges();
                }
            }

            return Json(new[] { formTemplateSelectionGroup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormTemplateSelectionGroup_Destroy([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.FormTemplateSelectionGroup formTemplateSelectionGroup)
        {
            if (formTemplateSelectionGroup != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                FormTemplateSelectionGroup formTemplateSelectionGroupInDb = clientDbContext.FormTemplateSelectionGroups
                    .Where(x => x.FormTemplateSelectionGroupId == formTemplateSelectionGroup.FormTemplateSelectionGroupId).SingleOrDefault();

                if (formTemplateSelectionGroupInDb != null)
                {
                    clientDbContext.FormTemplateSelectionGroups.Remove(formTemplateSelectionGroupInDb);
                    clientDbContext.SaveChanges();
                }
            }

            return Json(new[] { formTemplateSelectionGroup }.ToDataSourceResult(request, ModelState));
        }

        ////////////////////////////////////////////////////////////////////////

        public ActionResult Workflows()
        {

            return View();
        }


        public ActionResult Workflows_Read([DataSourceRequest]DataSourceRequest request)
        {
            try
            {

                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var workflows = clientDbContext.Workflows.OrderBy(e => e.WorkflowDescription).ToList();


                return Json(workflows.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }

            catch (Exception err)
            {
                return Json(new DataSourceResult
                {
                    Errors = err.Message
                });
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Workflow_Create([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.Workflow workflow)
        {

            if (workflow != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var workflowInDb = clientDbContext.Workflows
                    .Where(x => x.WorkflowName == workflow.WorkflowName)
                    .SingleOrDefault();

                if (workflowInDb != null)
                {
                    ModelState.AddModelError("", "The Workflow" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                }
                else
                {
                    var newWorkflow = new Workflow { WorkflowName = workflow.WorkflowName, WorkflowDescription = workflow.WorkflowDescription };
                    clientDbContext.Workflows.Add(newWorkflow);
                    clientDbContext.SaveChanges();
                    workflow.WorkflowId = newWorkflow.WorkflowId;
                }

            }

            return Json(new[] { workflow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Workflow_Update([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.Workflow workflow)
        {
            if (workflow != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var workflowInDb = clientDbContext.Workflows
                    .Where(x => x.WorkflowId == workflow.WorkflowId)
                    .SingleOrDefault();

                if (workflowInDb != null)
                {
                    workflowInDb.WorkflowName = workflow.WorkflowName;
                    workflowInDb.WorkflowDescription = workflow.WorkflowDescription;
                    clientDbContext.SaveChanges();
                }
            }

            return Json(new[] { workflow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Workflow_Destroy([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.Workflow workflow)
        {
            if (workflow != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                Workflow workflowInDb = clientDbContext.Workflows.Where(x => x.WorkflowId == workflow.WorkflowId).SingleOrDefault();

                if (workflowInDb != null)
                {
                    clientDbContext.Workflows.Remove(workflowInDb);
                    clientDbContext.SaveChanges();
                }
            }

            return Json(new[] { workflow }.ToDataSourceResult(request, ModelState));
        }

        //////////////////////////////// WORKFLOW MEMEBERS Starts Here //////////////////////////////////////////

        public ActionResult WorkflowMembers(string requestType)
        {
            // refactor this
            int nId = Convert.ToInt32(requestType);
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);

            PopulateUserGroupListForPotentialWorkflowMembers();

            string workflowName = clientDbContext.Workflows
                .Where(x => x.WorkflowId == nId).Select(x => x.WorkflowName)
                .SingleOrDefault();
            ViewBag.WorkflowName = workflowName;

            return View();
            
        }

        public ActionResult WorkflowMembers_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            try
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                List<WorkflowMemberVm> workflowMemberVmList = clientDbContext.WorkflowMembers.Where(x => x.WorkflowId == id)
                    .Select(x => new WorkflowMemberVm
                    {
                        WorkflowMemberId = x.WorkflowMemberId,
                        WorkflowId = x.WorkflowId,
                        UserOrGroupName = x.UserOrGroupName,
                        IsGroup = x.IsGroup,
                        Position = x.Position  
                    }).OrderBy(e => e.Position).ToList();

                return Json(workflowMemberVmList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                return Json(new DataSourceResult
                {
                    Errors = err.Message
                });
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkflowMember_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.WebUI.Models.WorkflowMemberVm workflowMember, int id)
        {

            if (workflowMember != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                //var workflowMemberInDb = clientDbContext.WorkflowMembers
                //    .Where(x => x.UserOrGroupName == workflowMember && x.WorkflowId == id)
                //    .SingleOrDefault();

                //if (workflowMemberInDb != null)
                //{
                //    ModelState.AddModelError("", "The Form Template Field HtmlId is already defined.");
                //}
                //else
                //{
                    var newWorkflowMember = new WorkflowMember
                    {
                        WorkflowId = id,
                        IsGroup = workflowMember.IsGroup,
                        Position = workflowMember.Position,
                        UserOrGroupName = workflowMember.UserOrGroupName
                    };

                    clientDbContext.WorkflowMembers.Add(newWorkflowMember);
                    clientDbContext.SaveChanges();
                    workflowMember.WorkflowMemberId = newWorkflowMember.WorkflowMemberId;
                //}

            }

            return Json(new[] { workflowMember }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkflowMember_Update([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.WorkflowMember workflowMember)
        {
            if (workflowMember != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var workflowMemberInDb = clientDbContext.WorkflowMembers
                    .Where(x => x.WorkflowMemberId == workflowMember.WorkflowMemberId)
                    .SingleOrDefault();

                if (workflowMemberInDb != null)
                {
                    // should check to see if this is really a group or not a group **************************

                    workflowMemberInDb.IsGroup = workflowMember.IsGroup;
                    workflowMemberInDb.Position = workflowMember.Position;
                    workflowMemberInDb.UserOrGroupName = workflowMember.UserOrGroupName;

                    clientDbContext.SaveChanges();
                }
            }

            return Json(new[] { workflowMember }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkflowMember_Destroy([DataSourceRequest] DataSourceRequest request, ExecViewHrk.EfClient.WorkflowMember workflowMember)
        {
            if (workflowMember != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                WorkflowMember workflowMemberInDb = clientDbContext.WorkflowMembers
                    .Where(x => x.WorkflowMemberId == workflowMember.WorkflowMemberId).SingleOrDefault();

                if (workflowMemberInDb != null)
                {
                    clientDbContext.WorkflowMembers.Remove(workflowMemberInDb);
                    clientDbContext.SaveChanges();
                }
            }

            return Json(new[] { workflowMember }.ToDataSourceResult(request, ModelState));
        }

        public class UserGroupNameEntry
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }
        private void PopulateUserGroupListForPotentialWorkflowMembers()
        {
            AdminDbContext adminDbContext = new AdminDbContext();
            
            var userGroupNameList = adminDbContext.AspNetRoles
                .Select(x => new UserGroupNameEntry { Name = x.Name, Description = x.Name }).OrderBy(x => x.Name).ToList();

            int currentClientId = Convert.ToInt32(User.Identity.GetSelectedClientID());

            int nHrkEmployerId = adminDbContext.Employers.Where(x => x.EmployerName == "HR Knowledge").Select(x => x.EmployerId).SingleOrDefault();
            int nResNavEmployerId = adminDbContext.Employers.Where(x => x.EmployerName == "Resource Navigation").Select(x => x.EmployerId).SingleOrDefault();
            
            var externalUsers = adminDbContext.ExternalUserClients.Include(x => x.AspNetUser)
                                    .Where(x => x.EmployerId == currentClientId).Select(x => x.AspNetUser.UserName);
            var usersMultipleCompanies = adminDbContext.UserCompanies.Include(x => x.AspNetUser)
                                    .Where(x => x.EmployerId == currentClientId).Select(x => x.AspNetUser.UserName);

            var userList = adminDbContext.AspNetUsers
                .Where(x => x.EmployerId == currentClientId || x.EmployerId == nHrkEmployerId || x.EmployerId == nResNavEmployerId
                    || externalUsers.Contains(x.UserName) || usersMultipleCompanies.Contains(x.UserName))
                .Select(x => new UserGroupNameEntry { Name = x.UserName, Description = x.UserName }).OrderBy(x => x.Name).ToList();

            userGroupNameList.AddRange(userList);

            ViewData["UserGroupNames"] = userGroupNameList;
            ViewData["defaultFieldType"] = userGroupNameList.First();
        }

        /////////////////////////////// FORM TEMPLATE WORKFLOWS STARTS HERE /////////////////////////////////////////

        public ActionResult FormTemplateWorkflows()
        {
            PopulateFormTemplateList();
            PopulateWorkflowList();
            return View();
        }


        public ActionResult FormTemplateWorkflows_Read([DataSourceRequest]DataSourceRequest request)
        {
            try
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var formTemplateWorkflows = clientDbContext.FormTemplateWorkflows.OrderBy(e => e.FormTemplateWorkflowName).ToList();

                return Json(formTemplateWorkflows.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }

            catch (Exception err)
            {
                return Json(new DataSourceResult
                {
                    Errors = err.Message
                });
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormTemplateWorkflow_Create([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.FormTemplateWorkflow formTemplateWorkflow)
        {

            if (formTemplateWorkflow != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var formTemplateWorkflowInDb = clientDbContext.FormTemplateWorkflows
                    .Where(x => x.FormTemplateWorkflowName == formTemplateWorkflow.FormTemplateWorkflowName)
                    .SingleOrDefault();

                if (formTemplateWorkflowInDb != null)
                {
                    ModelState.AddModelError("", "The Form Template Workflow" + CustomErrorMessages.ERROR_ALREADY_DEFINED);
                }
                else
                {
                    var newFormTemplateWorkflow = new FormTemplateWorkflow
                    {
                        FormTemplateWorkflowName = formTemplateWorkflow.FormTemplateWorkflowName,
                        FormTemplateId = formTemplateWorkflow.FormTemplateId,
                        WorkflowId = formTemplateWorkflow.WorkflowId
                    };

                    clientDbContext.FormTemplateWorkflows.Add(newFormTemplateWorkflow);

                    clientDbContext.SaveChanges();
                    formTemplateWorkflow.FormTemplateWorkflowId = newFormTemplateWorkflow.FormTemplateWorkflowId;

                    // now add records to FormWorkflowFieldPermissions for each workflow memeber/form field
                    List<FormTemplateField> formTemplateFields = clientDbContext.FormTemplateFields
                        .Where(x => x.FormTemplateId == formTemplateWorkflow.FormTemplateId).ToList();

                    List<WorkflowMember> workflowMembers = clientDbContext.WorkflowMembers
                        .Where(x => x.WorkflowId == formTemplateWorkflow.WorkflowId).ToList();

                    foreach (WorkflowMember workflowMember in workflowMembers)
                    {
                        foreach (FormTemplateField formTemplateField in formTemplateFields)
                        {
                            clientDbContext.FormWorkflowFieldPermissions
                                .Add(new FormWorkflowFieldPermission
                                {
                                    FormTemplateWorkflowId = formTemplateWorkflow.FormTemplateWorkflowId,
                                    WorlflowMemberId = workflowMember.WorkflowMemberId,
                                    FormTemplateFieldId = formTemplateField.FormTemplateFieldId,
                                    CanView = true,
                                    CanEdit = true
                                });
                        }
                    }

                    clientDbContext.SaveChanges();
                    formTemplateWorkflow.FormTemplateWorkflowId = newFormTemplateWorkflow.FormTemplateWorkflowId;
                }

            }

            return Json(new[] { formTemplateWorkflow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormTemplateWorkflow_Update([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.FormTemplateWorkflow formTemplateWorkflow)
        {
            if (formTemplateWorkflow != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var formTemplateWorkflowInDb = clientDbContext.FormTemplateWorkflows
                    .Where(x => x.FormTemplateWorkflowId == formTemplateWorkflow.FormTemplateWorkflowId)
                    .SingleOrDefault();

                if (formTemplateWorkflowInDb != null)
                {
                    formTemplateWorkflowInDb.FormTemplateWorkflowName = formTemplateWorkflow.FormTemplateWorkflowName;
                    clientDbContext.SaveChanges();
                }
            }

            return Json(new[] { formTemplateWorkflow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormTemplateWorkflow_Destroy([DataSourceRequest] DataSourceRequest request
            , ExecViewHrk.EfClient.FormTemplateWorkflow formTemplateWorkflow)
        {
            if (formTemplateWorkflow != null)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                FormTemplateWorkflow formTemplateWorkflowInDb = clientDbContext.FormTemplateWorkflows
                    .Where(x => x.FormTemplateWorkflowId == formTemplateWorkflow.FormTemplateWorkflowId).SingleOrDefault();

                if (formTemplateWorkflowInDb != null)
                {
                    clientDbContext.FormTemplateWorkflows.Remove(formTemplateWorkflowInDb);
                    clientDbContext.SaveChanges();
                }
            }

            return Json(new[] { formTemplateWorkflow }.ToDataSourceResult(request, ModelState));
        }


        /////////////////////////////// FORM TEMPLATE WORKFLOWS ENDS HERE /////////////////////////////////////////
        


        /// ///////////////////////////////////////////////////// Field Permission Starts HERE ///////////////////////
        
        public ActionResult FormWorkflowFieldPermissions_Read([DataSourceRequest]DataSourceRequest request, int formTemplateWorkflowId)
        {
            try
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                var formTemplateWorkflowPermissions = clientDbContext.FormWorkflowFieldPermissions
                    .Include(x => x.WorkflowMember.UserOrGroupName)
                    .Include(x => x.FormTemplateField.HtmlId)
                    .Include(x => x.FormTemplateField.Position)
                    .Where(x => x.FormTemplateWorkflowId == formTemplateWorkflowId).OrderBy(e => e.FormTemplateField.Position)
                    .Select(x => new FormWorkflowFieldPermissionVm
                    {
                        CanEdit = x.CanEdit,
                        CanView = x.CanView,
                        FormWorkflowFieldPermissionId = x.FormWorkflowFieldPermissionId,
                        FormTemplateWorkflowId = x.FormTemplateWorkflowId,
                        WorkflowMemberId = x.WorlflowMemberId,
                        FormTemplateFieldId = x.FormTemplateFieldId,
                        WorkflowMemberName = x.WorkflowMember.UserOrGroupName,
                        FieldName = x.FormTemplateField.HtmlId
                        
                    }).ToList();

                return Json(formTemplateWorkflowPermissions.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }

            catch (Exception err)
            {
                return Json(new DataSourceResult
                {
                    Errors = err.Message
                });
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormWorkflowFieldPermissions_Update([DataSourceRequest] DataSourceRequest request
            , [Bind(Prefix = "models")]IEnumerable<ExecViewHrk.WebUI.Models.FormWorkflowFieldPermissionVm> formWorkflowFieldPermissions)
            //, int formTemplateWorkflowId)
        {
            if (formWorkflowFieldPermissions != null && ModelState.IsValid)
            {
                string connString = User.Identity.GetClientConnectionString();
                ClientDbContext clientDbContext = new ClientDbContext(connString);

                foreach (var fieldPermission in formWorkflowFieldPermissions)
                {
                    var formWorkflowFieldPermission = new FormWorkflowFieldPermission();

                    formWorkflowFieldPermission.FormWorkflowFieldPermissionId = fieldPermission.FormWorkflowFieldPermissionId;
                    formWorkflowFieldPermission.FormTemplateWorkflowId = fieldPermission.FormTemplateWorkflowId;
                    formWorkflowFieldPermission.WorlflowMemberId = fieldPermission.WorkflowMemberId;
                    formWorkflowFieldPermission.FormTemplateFieldId = fieldPermission.FormTemplateFieldId;
                    formWorkflowFieldPermission.CanEdit = fieldPermission.CanEdit;
                    formWorkflowFieldPermission.CanView = fieldPermission.CanView;


                    clientDbContext.FormWorkflowFieldPermissions.Attach(formWorkflowFieldPermission);
                    clientDbContext.Entry(formWorkflowFieldPermission).State = EntityState.Modified;

                    clientDbContext.SaveChanges();

                }
                //clientDbContext.SaveChanges();
                
            }

            return Json(new[] { formWorkflowFieldPermissions }.ToDataSourceResult(request, ModelState));
        }

        private void PopulateFormTemplateList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var formTemplateNames = clientDbContext.FormTemplates
                .Select(x => new { x.FormTemplateName, x.FormTemplateId }).OrderBy(x => x.FormTemplateName).ToList();

            ViewData["FormTemplateList"] = formTemplateNames;
            ViewData["defaultFormTemplateId"] = formTemplateNames.First().FormTemplateId;
        }

        private void PopulateWorkflowList()
        {
            string connString = User.Identity.GetClientConnectionString();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var workflowNames = clientDbContext.Workflows
                .Select(x => new { x.WorkflowName, x.WorkflowId }).OrderBy(x => x.WorkflowName).ToList();
            
            ViewData["WorkflowList"] = workflowNames;
            ViewData["defaultWorkflowId"] = workflowNames.First().WorkflowId;
        }

    }
}