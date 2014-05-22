using Ninject.Modules;
using Service.Services;
using Service.Interfaces;

namespace Web.Infrastructure
{
    /// <summary>
    /// web modules registration
    /// </summary>
    public class WebModules : NinjectModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>


        public override void Load()
        {
            Bind<IAuditFileService>().To<AuditFileService>();
            Bind<IAuditService>().To<AuditService>();
            Bind<IBaseFormService>().To<BaseFormService>();
            Bind<IBusinessProcessService>().To<BusinessProcessService>();
            Bind<IClassificationService>().To<ClassificationService>();
            Bind<IClauseService>().To<ClauseService>();
            Bind<IDetailTypeService>().To<DetailTypeService>();
            Bind<ICorrectiveActionFileService>().To<CorrectiveActionFileService>();
            Bind<ICorrectiveActionNoteService>().To<CorrectiveActionNoteService>();
            Bind<ICorrectiveActionRequestService>().To<CorrectiveActionRequestService>();
            Bind<ICustomerFeedbackService>().To<CustomerFeedbackService>();
            Bind<ICustomerFeedbackFileService>().To<CustomerFeedbackFileService>();
            Bind<ICustomerService>().To<CustomerService>();
            Bind<IDailyOperationFileService>().To<DailyOperationFileService>();
            Bind<IDailyOperationService>().To<DailyOperationService>();
            Bind<IDepartmentService>().To<DepartmentService>();
            Bind<IDiscoveryService>().To<DiscoveryService>();    
            Bind<IDispositionTypeService>().To<DispositionTypeService>();
            Bind<IEmailService>().To<EmailService>();
            Bind<IInvalidReasonService>().To<InvalidReasonService>();
            Bind<IImprovementTypeService>().To <ImprovementTypeService>();
            Bind<IIsoActionService>().To<IsoActionService>();
            Bind<ILoginService>().To<LoginService>();
            Bind<IMachineService>().To<MachineService>();
            Bind<IManagementReviewService>().To<ManagementReviewService>();
            Bind<IManagementReviewFileService>().To<ManagementReviewFileService>();
            Bind<IOperatorService>().To<OperatorService>();
            Bind<IPartService>().To<PartService>();
            Bind<IPreventiveActionFileService>().To<PreventiveActionFileService>();
            Bind<IPreventiveActionNoteService>().To<PreventiveActionNoteService>();
            Bind<IPreventiveActionRequestService>().To<PreventiveActionRequestService>();
            Bind<IProductDiscrepancyService>().To<ProductDiscrepancyService>();
            Bind<IProductDiscrepancyFileService>().To<ProductDiscrepancyFileService>();
            Bind<IRejectionCodesService>().To<RejectionCodesService>();
            Bind<IRejectionDiscoveryService>().To<RejectionDiscoveryService>();
            Bind<IRejectedPartService>().To<RejectedPartService>();
            Bind<IReportsService>().To<ReportsService>();
            Bind<IResultingFromService>().To<ResultingFromService>();
            Bind<IRootCauseService>().To<RootCauseService>();
            Bind<ISearchService>().To<SearchService>();
            Bind<ISubClauseService>().To<SubClauseService>();
            Bind<ISupplierService>().To<SupplierService>();
            Bind<ISubscriberService>().To<SubscriberService>();
            Bind<ISubscriberLogoService>().To<SubscriberLogoService>();
            Bind<IUserService>().To<UserService>();
            Bind<IWorkCenterService>().To<WorkCenterService>();
        }
    } // class
} // namespace