using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core;
using Core.Domains;
using Service.Services;
using Web.Infrastructure;
using ResultingFrom = Core.Domains.ResultingFrom;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace Web.Helpers {
    public static class ListHelper
    {
        #region HTML helpers
        public static MvcHtmlString CascadingDropDownListFor<TModel, TProperty>(
           this HtmlHelper<TModel> htmlHelper,
           Expression<Func<TModel, TProperty>> expression,
           IEnumerable<SelectListItem> selectList,
           string optionLabel,
           object htmlAttributes,
           string parentControlName,
           string childListUrl
           ) {
            var memberName = GetMemberInfo(expression).Member.Name;
            var returnHtml = System.Web.Mvc.Html.SelectExtensions.DropDownListFor(htmlHelper, expression, selectList, optionLabel, htmlAttributes);
            var returnString = MvcHtmlString.Create(returnHtml +
                    @"<script type=""text/javascript"">
                        $(document).ready(function () {
                            $(""#<<parentControlName>>"").change(function () { 
                                var postData = { <<parentControlName>>: $(""#<<parentControlName>>"").val() };
                                $.post('<<childListUrl>>', postData, function (data) {
                                    var y = $('#subcat').val();
                                    var options = """";
                                    $.each(data, function (index) {
                                        if (data[index].Id == y){
                                            options += ""<option selected='selected' value='"" + data[index].Id + ""'>"" + data[index].Name + ""</option>"";
                                        } else {
                                            options += ""<option value='"" + data[index].Id + ""'>"" + data[index].Name + ""</option>"";
                                        }
                                    });
                                    $(""#<<memberName>>"").html(options);
                                })
                                .error(function (jqXHR, textStatus, errorThrown) { alert(jqXHR.responseText); });
                            });
                        });
                     </script>"
                    .Replace("<<parentControlName>>", parentControlName)
                    .Replace("<<childListUrl>>", childListUrl)
                    .Replace("<<memberName>>", memberName));

            return returnString;
        }
        #endregion

        #region Enum Helpers
        /// <summary>
        /// Gets the list of Roles based off current user
        /// </summary>
        /// <returns>List of Roles</returns>
        private static string DisplayName(this Enum value) {
            var enumType = value.GetType();
            var enumValue = Enum.GetName(enumType, value);
            var member = enumType.GetMember(enumValue)[0];

            var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            var outString = ((DisplayAttribute)attrs[0]).Name;

            if (((DisplayAttribute)attrs[0]).ResourceType != null) {
                outString = ((DisplayAttribute)attrs[0]).GetName();
            }

            return outString;
        }
        #endregion

        #region Get Singles
        /// <summary>
        /// Gets the business process by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static BusinessProcess GetBusinessProcessById(int id) {
            var businessProcessService = new BusinessProcessService();
            return businessProcessService.GetBusinessProcessById(id);
        }

        /// <summary>
        /// Gets the classification by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Classification GetClassificationById(int id) {
            var classificationService = new ClassificationService();
            return classificationService.GetClassificationById(id);
        }

        /// <summary>
        /// Gets the department by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Department GetDepartmentById(int id) {
            var departmentService = new DepartmentService();
            return departmentService.GetDepartmentById(id);
        }

        /// <summary>
        /// Gets the detail type by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static DetailType GetDetailTypeById(int id)
        {
            var complaintTypeService = new DetailTypeService();
            return complaintTypeService.GetDetailTypeById(id);
        }

        /// <summary>
        /// Gets the discovery type by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Discovery GetDiscoveryTypeById(int id) {
            var discoveryService = new DiscoveryService();
            return discoveryService.GetDiscoveryById(id);
        }

        /// <summary>
        /// Gets the external type by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static ExternalType GetExternalTypeById(int id) {
            var externalTypeService = new ExternalTypeService();
            return externalTypeService.GetExternalTypeById(id);
        }

        /// <summary>
        /// Gets the improvement type by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static ImprovementType GetImprovementTypeById(int id)
        {
            var improvementTypeService = new ImprovementTypeService();
            return improvementTypeService.GetImprovementTypeById(id);
        }

        /// <summary>
        /// Gets the member info.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">method</exception>
        /// <exception cref="System.ArgumentException">method</exception>
        private static MemberExpression GetMemberInfo(Expression method)
        {
            var lambda = method as LambdaExpression;
            if (lambda == null)
                throw new ArgumentNullException("method");

            MemberExpression memberExpr = null;

            switch (lambda.Body.NodeType)
            {
                case ExpressionType.Convert:
                    memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    break;
                case ExpressionType.MemberAccess:
                    memberExpr = lambda.Body as MemberExpression;
                    break;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");

            return memberExpr;
        }

        /// <summary>
        /// Gets the root cause by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static RootCause GetRootCauseById(int id)
        {
            var rootCauseService = new RootCauseService();
            return rootCauseService.GetRootCauseById(id);
        }

        /// <summary>
        /// Gets the subscriber logo by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static SubscriberLogo GetSubscriberLogoById(int id)
        {
            var subscriberLogoService = new SubscriberLogoService();
            return subscriberLogoService.GetSubscriberLogoById(id);
        }

        /// <summary>
        /// Gets the root cause by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        //public static SubscriberLogo GetSubscriberLogoById(int id) {
        //    var subscriberLogoService = new SubscriberLogoService();
        //    return subscriberLogoService.GetSubscriberLogoById(id);
        //}
        #endregion

        #region Get Lists
        /// <summary>
        /// Gets the list of Business Processes
        /// </summary>
        /// <returns>List of Business Processes</returns>
        public static List<BusinessProcess> GetListOfBusinessProcesses(int? id) {
            var businessProcessService = new BusinessProcessService();
            var businessProcessList = businessProcessService.GetAll().Where(x => (x.Active)).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                businessProcessList = businessProcessList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }
            if (id.HasValue && id != 0) {
                var additional = businessProcessService.GetBusinessProcessById(id.Value);
                businessProcessList.Add(additional);
            }
            return businessProcessList.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of classifications.
        /// </summary>
        /// <returns>List of Classifications</returns>
        public static List<Classification> GetListOfClassifications(int? id) {
            var classificationService = new ClassificationService();
            var classificationList = classificationService.GetAll().Where(x => x.Active).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                classificationList = classificationList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }

            if (id.HasValue && id != 0) {
                var additionalClassification = classificationService.GetClassificationById(id.Value);
                classificationList.Add(additionalClassification);
            }
            return classificationList.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of clauses.
        /// </summary>
        /// <returns></returns>
        public static List<Clause> GetListOfClauses() {
            var clauseService = new ClauseService();
            var clauseList = clauseService.GetAll().Where(x => x.Active).ToList();
            return clauseList.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of customers.
        /// </summary>
        /// <returns></returns>
        public static List<Customer> GetListOfCustomers(int id, bool createNew = true) {
            var customerService = new CustomerService();
            var customerList = customerService.GetAll().Where(x => x.Active).ToList();

            if (SessionDataHelper.SubscriberId != 0) {
                customerList = customerList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }

            if (id != 0) {
                var additionalCustomer = customerService.GetCustomerById(id);
                customerList.Add(additionalCustomer);
            }
            if (createNew) {
                var newcustomer = new Customer {
                    Id = -1,
                    Name = "-- Create Customer -"
                };
                customerList.Add(newcustomer);
            }
            return customerList.Distinct().OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Gets the list of departments.
        /// </summary>
        /// <returns>List of Departments</returns>
        public static List<Department> GetListOfDepartments(int? id) {
            var departmentService = new DepartmentService();
            var departmentList = departmentService.GetAll().Where(x => x.Active).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                departmentList = departmentList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }

            if (id.HasValue && id != 0) {
                var additional = departmentService.GetDepartmentById(id.Value);
                departmentList.Add(additional);
            }
            return departmentList.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of complaint types.
        /// </summary>
        /// <returns>List of Complaint Types</returns>
        public static List<DetailType> GetListOfDetailTypes(int? id) {
            var complaintTypeService = new DetailTypeService();
            var complaintTypeList = complaintTypeService.GetAll().Where(x => x.Active).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                complaintTypeList = complaintTypeList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }

            if (id.HasValue && id != 0) {
                var additionalDetailType = complaintTypeService.GetDetailTypeById(id.Value);
                complaintTypeList.Add(additionalDetailType);
            }
            return complaintTypeList.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of discovery types.
        /// </summary>
        /// <returns></returns>
        public static List<Discovery> GetListOfDiscoveryTypes(int? id) {
            var discoveryService = new DiscoveryService();
            var discoveryTypeList = discoveryService.GetAll().Where(x => x.Active).ToList();

            if (SessionDataHelper.SubscriberId != 0) {
                discoveryTypeList = discoveryTypeList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }

            if (id.HasValue && id != 0) {
                var additional = discoveryService.GetDiscoveryById(id.Value);
                discoveryTypeList.Add(additional);
            }
            return discoveryTypeList.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of disposition types.
        /// </summary>
        /// <returns></returns>
        public static List<DispositionType> GetListOfDispositionTypes(int? id) {
            var dispositionTypeService = new DispositionTypeService();
            var dispositionTypeList = dispositionTypeService.GetAll().Where(x => x.Active).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                dispositionTypeList =
                    dispositionTypeList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }
            if (id.HasValue && id != 0) {
                var additional = dispositionTypeService.GetDispositionTypeById(id.Value);
                dispositionTypeList.Add(additional);
            }
            return dispositionTypeList.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of employees.
        /// </summary>
        /// <returns>List of Employees</returns>
        public static List<User> GetListOfEmployees(int? id, Role role, bool active, bool all = false) {
            var userService = new UserService();
            var employeeList = userService.GetAll().ToList();

            if (!all) {
                employeeList = userService.GetAll().Where(x => x.Role == role).ToList();
            }

            if (!active) {
                employeeList = employeeList.Where(x => x.IsActive).ToList();
            }

            if (SessionDataHelper.SubscriberId != 0) {
                employeeList = employeeList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }

            if (id.HasValue && id != 0) {
                var additionalUser = userService.GetById(id.Value);
                employeeList.Add(additionalUser);
            }
            return employeeList.Distinct().OrderBy(x => x.FullName).ToList();
        }

        /// <summary>
        /// Gets the list of externalTypes.
        /// </summary>
        /// <returns>List of ExternalTypes</returns>
        public static List<ExternalType> GetListOfExternalTypes(int? id)
        {
            var externalTypeService = new ExternalTypeService();
            var externalTypeList = externalTypeService.GetAll().Where(x => x.Active).ToList();
            //if (SessionDataHelper.SubscriberId != 0)
            //{
            //    externalTypeList = externalTypeList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            //}

            if (id.HasValue && id != 0)
            {
                var additional = externalTypeService.GetExternalTypeById(id.Value);
                externalTypeList.Add(additional);
            }
            return externalTypeList.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of improvement types.
        /// </summary>
        /// <returns></returns>
        public static List<ImprovementType> GetListOfImprovementTypes(int? id) {
            var improvementTypeService = new ImprovementTypeService();
            var improvementTypeList = improvementTypeService.GetAll().Where(x => x.Active).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                improvementTypeList = improvementTypeList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }
            if (id.HasValue && id != 0) {
                var additional = improvementTypeService.GetImprovementTypeById(id.Value);
                improvementTypeList.Add(additional);
            }
            return improvementTypeList.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of invalid reasons.
        /// </summary>
        /// <returns></returns>
        public static List<InvalidReason> GetListOfInvalidReasons(int? id) {
            var invalidReasonService = new InvalidReasonService();
            var invalidReasonList = invalidReasonService.GetAll().Where(x => x.Active).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                invalidReasonList = invalidReasonList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }
            if (id.HasValue && id != 0) {
                var additional = invalidReasonService.GetInvalidReasonById(id.Value);
                invalidReasonList.Add(additional);
            }
            return invalidReasonList.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of root causes.
        /// </summary>
        /// <returns>List of Root Causes</returns>
        public static List<IsoAction> GetListOfIsoActions(int? id) {
            var isoActionService = new IsoActionService();
            var isoActionList = isoActionService.GetAll().Where(x => x.Active).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                isoActionList = isoActionList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }

            if (id.HasValue && id != 0) {
                var additional = isoActionService.GetIsoActionById(id.Value);
                isoActionList.Add(additional);
            }
            return isoActionList.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of machines.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static List<Machine> GetListOfMachines(int? id) {
            var service = new MachineService();
            var list = service.GetAll().Where(x => x.Active).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                list = list.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }

            if (id.HasValue && id != 0) {
                var additional = service.GetMachineById(id.Value);
                list.Add(additional);
            }
            return list.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of operators.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static List<Operator> GetListOfOperators(int? id) {
            var service = new OperatorService();
            var list = service.GetAll().Where(x => x.Active).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                list = list.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }

            if (id.HasValue && id != 0) {
                var additional = service.GetOperatorById(id.Value);
                list.Add(additional);
            }
            return list.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of parts.
        /// </summary>
        /// <returns></returns>
        public static List<Part> GetListOfParts(bool createNew) {
            var partService = new PartService();
            var partsList = partService.GetAll().Where(x => x.Active).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                partsList = partsList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }
            if (createNew) {
                var newpart = new Part {
                    Id = -1,
                    PartName = "-- Create Part -"
                };
                partsList.Add(newpart);
            }
            return partsList.Distinct().OrderBy(x => x.PartName).ToList();
        }

        /// <summary>
        /// Gets the list of rejection codes.
        /// </summary>
        /// <returns></returns>
        public static List<RejectionCode> GetListOfRejectionCodes(int? id) {
            var rejectionCodeService = new RejectionCodesService();
            var rejectionCodesList = rejectionCodeService.GetAll().Where(x => x.Active).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                rejectionCodesList = rejectionCodesList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }
            if (id.HasValue && id != 0) {
                var additional = rejectionCodeService.GetRejectionCodeById(id.Value);
                rejectionCodesList.Add(additional);
            }
            return rejectionCodesList.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of rejection discoveries.
        /// </summary>
        /// <returns></returns>
        public static List<RejectionDiscovery> GetListOfRejectionDiscoveries(int? id) {
            var rejectionDiscoveryService = new RejectionDiscoveryService();
            var rejectionDiscoveryList = rejectionDiscoveryService.GetAll().Where(x => x.Active).ToList();
            if (id.HasValue && id != 0) {
                var additional = rejectionDiscoveryService.GetRejectionDiscoveryById(id.Value);
                rejectionDiscoveryList.Add(additional);
            }
            return rejectionDiscoveryList.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of resulting from.
        /// </summary>
        /// <returns>List of Resulting Froms</returns>
        public static List<ResultingFrom> GetListOfResultingFrom() {
            var resultingFromService = new ResultingFromService();
            var resultingFromList = resultingFromService.GetAll().ToList();
            return resultingFromList;
        }

        /// <summary>
        /// Gets the list of root causes.
        /// </summary>
        /// <returns>List of Root Causes</returns>
        public static List<RootCause> GetListOfRootCauses(int? id) {
            var rootCauseService = new RootCauseService();
            var rootCauseList = rootCauseService.GetAll().Where(x => x.Active).ToList();

            if (SessionDataHelper.SubscriberId != 0) {
                rootCauseList = rootCauseList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }

            if (id.HasValue && id != 0) {
                var additional = rootCauseService.GetRootCauseById(id.Value);
                rootCauseList.Add(additional);
            }
            return rootCauseList.Distinct().OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Gets the list of sub clauses.
        /// </summary>
        /// <returns></returns>
        public static List<Core.Domains.SubClause> GetListOfSubClauses() {
            var subClauseService = new SubClauseService();
            var subClauseList = subClauseService.GetAll().Where(x => x.Active).ToList();
            return subClauseList;
        }

        /// <summary>
        /// Gets the list of subscribers.
        /// </summary>
        /// <returns></returns>
        public static List<Subscriber> GetListOfSubscribers(bool admin = false) {
            var subscriberService = new SubscriberService();

            var subscriberServiceList = subscriberService.GetAll().Where(x => x.Active);
            //if (admin)
            //{
            //    var _userService = new UserService();
            //    subscriberServiceList.ToList().Add(_userService.GetById(1));
            //}
            return subscriberServiceList.OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Gets the list of supplier.
        /// </summary>
        /// <returns>List of Suppliers</returns>
        public static List<Supplier> GetListOfSuppliers(int? id, bool createNew = true) {
            var supplierService = new SupplierService();
            var supplierList = supplierService.GetAllSupplier().Where(x => x.Active).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                supplierList = supplierList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }

            if (id.HasValue && id != 0) {
                var additionalSupplier = supplierService.GetById(id.Value);
                supplierList.Add(additionalSupplier);
            }
            if (createNew) {
                var newsupplier = new Supplier {
                    Id = -1,
                    Name = "-- Create Supplier -"
                };
                supplierList.Add(newsupplier);
            }
            return supplierList.Distinct().OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Gets the list of users.
        /// </summary>
        /// <returns></returns>
        public static List<User> GetListOfUsers() {
            var userService = new UserService();
            var userList = userService.GetAll().Where(x => x.IsActive && x.Role != Role.SystemAdmin).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                userList = userList.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }
            return userList.Distinct().OrderBy(x => x.FullName).ToList();
        }

        /// <summary>
        /// Gets all status.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetListOfSearches()
        {
            return new Dictionary<string, string>
                {
                    {"Audit", "Audit"},
                    {"CustomerFeedback", "Customer Feedback"},
                    {"DailyOperation", "Daily Operation"},
                    {"ManagementReview", "Management Review"},
                    {"ProductDiscrepancy", "Product Discrepancy"},
                    {"CorrectiveAction", "Corrective Action Request"},
                    {"PreventiveAction", "Preventive Action Request"}
                };
        }

        /// <summary>
        /// Gets the list of work centers.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static List<WorkCenter> GetListOfWorkCenters(int? id) {
            var service = new WorkCenterService();
            var list = service.GetAll().Where(x => x.Active).ToList();
            if (SessionDataHelper.SubscriberId != 0) {
                list = list.Where(x => x.SubscriberId == SessionDataHelper.SubscriberId).ToList();
            }

            if (id.HasValue && id != 0) {
                var additional = service.GetWorkCenterById(id.Value);
                list.Add(additional);
            }
            return list.Distinct().OrderBy(x => x.Description).ToList();
        }
        #endregion

        #region Get Dictionaries
        /// <summary>
        /// Gets all CAR status.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> GetAllCARStatus() {
            return new Dictionary<int, string>
            {
                {(int)CorrectiveActionTypes.Internal, CorrectiveActionTypes.Internal.DisplayName()},
                {(int)CorrectiveActionTypes.External, CorrectiveActionTypes.External.DisplayName()},
            };
        }

        /// <summary>
        /// Gets all status.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Status, string> GetAllStatus() {
            return new Dictionary<Status, string>
                {
                    {Status.Saved, Status.Saved.DisplayName()},
                    {Status.Issued, Status.Issued.DisplayName()},
                    {Status.Response, Status.Response.DisplayName()},
                    {Status.PreventiveAction, Status.PreventiveAction.DisplayName()},
                    {Status.CorrectiveAction, Status.CorrectiveAction.DisplayName()},
                    {Status.ProductDiscrepancy, Status.ProductDiscrepancy.DisplayName()},
                    {Status.PendingFollowUp, Status.PendingFollowUp.DisplayName()},
                    {Status.Closed, Status.Closed.DisplayName()},
                    {Status.Deleted, Status.Deleted.DisplayName()},
                };
        }

        /// <summary>
        /// Gets the form status.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> GetFormStatus() {
            return new Dictionary<int, string>
            {
                {(int)Status.CorrectiveAction, Status.CorrectiveAction.DisplayName()},
                {(int)Status.PreventiveAction, Status.PreventiveAction.DisplayName()},
                //{(int)Status.Closed, Status.Closed.DisplayName()},
            };
        }

        /// <summary>
        /// Gets the type of the list audit.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<AuditTypes, string> GetListAuditType() {
            return new Dictionary<AuditTypes, string>
                {
                    {AuditTypes.Customer, AuditTypes.Customer.DisplayName()},
                    {AuditTypes.External, AuditTypes.External.DisplayName()},
                    {AuditTypes.Internal, AuditTypes.Internal.DisplayName()},
                };
        }

        /// <summary>
        /// Gets the type of the list cost.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Cost, string> GetListCost() {
            return new Dictionary<Cost, string>
                {
                    {Cost.Insignificant, Cost.Insignificant.DisplayName()},
                    {Cost.Moderate, Cost.Moderate.DisplayName()},
                    {Cost.Significant, Cost.Significant.DisplayName()},
                };
        }

        /// <summary>
        /// Gets the list discrepency types.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<DiscrepencyType, string> GetListDiscrepencyTypes()
        {
            return Enum.GetValues(typeof(DiscrepencyType)).Cast<DiscrepencyType>().ToDictionary(val => val, val => val.DisplayName());
        }

        /// <summary>
        /// Gets the list discrepency types.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Country, string> GetListCountries()
        {
            return Enum.GetValues(typeof(Country)).Cast<Country>().ToDictionary(val => val, val => val.DisplayName());
            //return new Dictionary<Country,string> {
            //    {
            //        Country.Afghanistan, Country.Afghanistan
            //    }
            //}
        }

        /// <summary>
        /// Gets the list feedback types.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<FeedbackTypes, string> GetListFeedbackTypes()
        {
            return Enum.GetValues(typeof(FeedbackTypes)).Cast<FeedbackTypes>().ToDictionary(val => val, val => val.DisplayName());
        }

        /// <summary>
        /// Gets the list of form types.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<FormTypes, string> GetListOfFormTypes()
        {
            return new Dictionary<FormTypes, string>
                {
                    {FormTypes.Audits, FormTypes.Audits.DisplayName()},
                    {FormTypes.CorrectiveActionRequest, FormTypes.CorrectiveActionRequest.DisplayName()},
                    {FormTypes.CustomerFeedback, FormTypes.CustomerFeedback.DisplayName()},
                    {FormTypes.DailyOperation, FormTypes.DailyOperation.DisplayName()},
                    {FormTypes.ManagementReview, FormTypes.ManagementReview.DisplayName()},
                    {FormTypes.PreventiveActionRequest, FormTypes.PreventiveActionRequest.DisplayName()},
                    {FormTypes.ProductDiscrepancy, FormTypes.ProductDiscrepancy.DisplayName()}
                };
        }

        /// <summary>
        /// Gets the list of generic types.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<GenericTypes, string> GetListOfGenericTypes()
        {
            return new Dictionary<GenericTypes, string>
                {
                    {GenericTypes.Customer, GenericTypes.Customer.DisplayName()},
                    {GenericTypes.External, GenericTypes.External.DisplayName()},
                    {GenericTypes.Internal, GenericTypes.Internal.DisplayName()},
                    {GenericTypes.ReturnMatieralAuthorization, GenericTypes.ReturnMatieralAuthorization.DisplayName()},
                };
        }

        /// <summary>
        /// Gets the list standards.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<GraphTypes, string> GetListOfGraphTypes()
        {
            var dict = new Dictionary<GraphTypes, string>
                {
                    {GraphTypes.Action, GraphTypes.Action.DisplayName()},
                    {GraphTypes.BusinessProcess, GraphTypes.BusinessProcess.DisplayName()},
                    {GraphTypes.Classification, GraphTypes.Classification.DisplayName()},
                    {GraphTypes.Clause, GraphTypes.Clause.DisplayName()},
                    {GraphTypes.DetailType, GraphTypes.DetailType.DisplayName()},
                    {GraphTypes.Customer, GraphTypes.Customer.DisplayName()},
                    {GraphTypes.Department, GraphTypes.Department.DisplayName()},
                    {GraphTypes.Discovery, GraphTypes.Discovery.DisplayName()},
                    {GraphTypes.DispositionType, GraphTypes.DispositionType.DisplayName()},
                    {GraphTypes.Forms, GraphTypes.Forms.DisplayName()},
                    {GraphTypes.ImprovementType, GraphTypes.ImprovementType.DisplayName()},
                    {GraphTypes.Machine, GraphTypes.Machine.DisplayName()},
                    {GraphTypes.Operator, GraphTypes.Operator.DisplayName()},
                    {GraphTypes.Part, GraphTypes.Part.DisplayName()},
                    {GraphTypes.RejectionCode, GraphTypes.RejectionCode.DisplayName()},
                    {GraphTypes.RejectionDiscovery, GraphTypes.RejectionDiscovery.DisplayName()},
                    {GraphTypes.RootCause, GraphTypes.RootCause.DisplayName()},
                    {GraphTypes.Status, GraphTypes.Status.DisplayName()},
                    {GraphTypes.Supplier, GraphTypes.Supplier.DisplayName()},
                    {GraphTypes.User, GraphTypes.User.DisplayName()},
                    {GraphTypes.WorkCenter, GraphTypes.WorkCenter.DisplayName()},
                };
            if (SessionDataHelper.SubscriberId == 0)
            {
                dict.Add(GraphTypes.Subscriber, GraphTypes.Subscriber.DisplayName());
            }
            return dict;
        }

        /// <summary>
        /// Gets the list of investigation results.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> GetListOfInvestigationResults()
        {
            var result = new Dictionary<int, string>
                {
                    {(int) InvestigationResults.Valid, InvestigationResults.Valid.DisplayName()},
                    {(int) InvestigationResults.Invalid, InvestigationResults.Invalid.DisplayName()}
                };

            return result;
        }

        /// <summary>
        /// Gets the resulting types.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<ResultingFromTypes, string> GetResultingTypes()
        {
            return new Dictionary<ResultingFromTypes, string>
                {
                    {ResultingFromTypes.CustomerAudit, ResultingFromTypes.CustomerAudit.DisplayName()},
                    {ResultingFromTypes.CustomerFeedback, ResultingFromTypes.CustomerFeedback.DisplayName()},
                    {ResultingFromTypes.DailyOperations, ResultingFromTypes.DailyOperations.DisplayName()},
                    {ResultingFromTypes.ExternalAudit, ResultingFromTypes.ExternalAudit.DisplayName()},
                    {ResultingFromTypes.InternalAudit, ResultingFromTypes.InternalAudit.DisplayName()},
                    {ResultingFromTypes.ManagementReview, ResultingFromTypes.ManagementReview.DisplayName()},
                    {ResultingFromTypes.ProductDiscrepancy, ResultingFromTypes.ProductDiscrepancy.DisplayName()},
                };
        }

        /// <summary>
        /// Gets the list roles.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Role, string> GetListRoles() {
            switch(SessionDataHelper.UserRole){
                case Role.SystemAdmin: {
                    return new Dictionary<Role,string>{
                        {Role.Employee, Role.Employee.DisplayName()},
                        {Role.Manager, Role.Manager.DisplayName()},
                        {Role.SystemAdmin, Role.SystemAdmin.DisplayName()}
                    };
                }
                case Role.Subscriber: {
                    return new Dictionary<Role, string>{
                           {Role.Employee, Role.Employee.DisplayName()},
                           {Role.Manager, Role.Manager.DisplayName()},
                           {Role.Supplier, Role.Supplier.DisplayName()},
                           {Role.Subscriber, Role.Subscriber.DisplayName()}
                       };
                }
                case Role.SystemAdminSubscriber:
                    {
                        return new Dictionary<Role, string>{
                           {Role.Employee, Role.Employee.DisplayName()},
                           {Role.Manager, Role.Manager.DisplayName()},
                           {Role.Supplier, Role.Supplier.DisplayName()},
                           {Role.Subscriber, Role.Subscriber.DisplayName()}
                       };
                    }
                default: {
                    return new Dictionary<Role, string>{
                        {Role.Employee, Role.Employee.DisplayName()},
                        {Role.Manager, Role.Manager.DisplayName()},
                        {Role.Supplier, Role.Supplier.DisplayName()},
                    };
                }
            }
            //return SessionDataHelper.UserRole == Role.SystemAdmin
            //           ? new Dictionary<Role, string>{
            //               {Role.Employee, Role.Employee.DisplayName()},
            //               {Role.Manager, Role.Manager.DisplayName()},
            //               {Role.SystemAdmin, Role.SystemAdmin.DisplayName()}
            //           }
            //           : new Dictionary<Role, string>{
            //               {Role.Employee, Role.Employee.DisplayName()},
            //               {Role.Manager, Role.Manager.DisplayName()},
            //               {Role.Supplier, Role.Supplier.DisplayName()}
            //           };
        }

        /// <summary>
        /// Gets the list standards.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Standard, string> GetListStandards() {
            return new Dictionary<Standard, string>
                {
                    {Standard.As9100, Standard.As9100.DisplayName()},
                    {Standard.As9120, Standard.As9120.DisplayName()},
                    {Standard.Iso9001, Standard.Iso9001.DisplayName()},
                    {Standard.Other, Standard.Other.DisplayName()},
                };
        }

        /// <summary>
        /// Gets the list status.
        /// </summary>
        /// <param name="formStatus">The form status.</param>
        /// <param name="elevateEligible">if set to <c>true</c> [elevate eligible].</param>
        /// <param name="elevating">if set to <c>true</c> [elevating].</param>
        /// <param name="ableToReject"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetListStatus(Status formStatus = 0, bool elevateEligible = false, bool elevating = false, bool ableToReject = false, bool pd = false)
        {
            // Local Variables
            var result = new Dictionary<int, string>();
            if (SessionDataHelper.UserRole <= Role.Subscriber)
            {
                //result.Add((int)Status.Deleted, Status.Deleted.DisplayName());
                //result.Add((int)Status.Invalid, Status.Invalid.DisplayName());
            }
            if (elevating)
            {
                result.Add((int)Status.Saved, Status.Saved.DisplayName());
                result.Add((int)Status.Issued, Status.Issued.DisplayName());
            }
            else
            {
                switch (formStatus)
                {
                    case Status.Saved:
                        {
                            result.Add((int)Status.Saved, Status.Saved.DisplayName());
                            result.Add((int)Status.Issued, Status.Issued.DisplayName());
                            result.Add((int)Status.Closed, Status.Closed.DisplayName());
                            break;
                        }
                    case Status.Issued:
                        {
                            result.Add((int)Status.Issued, Status.Issued.DisplayName());
                            result.Add((int)Status.Response, Status.Response.DisplayName());
                            result.Add((int)Status.Closed, Status.Closed.DisplayName());
                            break;
                        }
                    case Status.Response:
                        {
                            result.Add((int)Status.Response, Status.Response.DisplayName());
                            if (ableToReject && (int)formStatus >= (int)Status.Response)
                            {
                                result.Add((int)Status.PendingFollowUp, "Accept (Agree to this Action)");
                                result.Add((int)Status.Issued, "Reject (Disagree with this Action)");
                            }
                            result.Add((int)Status.Closed, Status.Closed.DisplayName());
                            break;
                        }
                    case Status.PendingFollowUp:
                        {
                            result.Add((int)Status.PendingFollowUp, Status.PendingFollowUp.DisplayName());
                            if (ableToReject && (int)formStatus >= (int)Status.Response)
                            {
                                result.Add((int)Status.Issued, "Reject (Disagree with this Action)");
                            }
                            result.Add((int)Status.Closed, Status.Closed.DisplayName());
                            break;
                        }
                    case Status.Closed:
                        {
                            result.Add((int)Status.Closed, Status.Closed.DisplayName());
                            break;
                        }
                    default:
                        {
                            result.Add((int)Status.Saved, Status.Saved.DisplayName());
                            break;
                        }
                }
            }

            if (elevateEligible)
            {
                result.Add((int)Status.CorrectiveAction, Status.CorrectiveAction.DisplayName());
                result.Add((int)Status.PreventiveAction, Status.PreventiveAction.DisplayName());
                if (pd)
                {
                    result.Add((int)Status.ProductDiscrepancy, Status.ProductDiscrepancy.DisplayName());
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the list status minimized.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> GetListStatusMinimized(bool getResponse = false)
        {
            // Local Variables
            var result = new Dictionary<int, string>
                {
                    {(int) Status.Issued, Status.Issued.DisplayName()},
                };
            if (getResponse)
            {
                result.Add((int) Status.Response, Status.Response.DisplayName());
            }
            result.Add((int) Status.Closed, Status.Closed.DisplayName());
            return result;
        }

        /// <summary>
        /// Gets the type of the list cost.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<TimeRequired, string> GetListTimeRequired()
        {
            return new Dictionary<TimeRequired, string>
                {
                    {TimeRequired.ZeroToOneMonth, TimeRequired.ZeroToOneMonth.DisplayName()},
                    {TimeRequired.ZeroToSixMonth, TimeRequired.ZeroToSixMonth.DisplayName()},
                    {TimeRequired.SixPlusMonth, TimeRequired.SixPlusMonth.DisplayName()},
                };
        }
        #endregion
    }
}