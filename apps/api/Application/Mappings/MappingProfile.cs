using AutoMapper;
using BetterTests.Application.DTOs;
using BetterTests.Domain.Entities;

namespace BetterTests.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Project, ProjectResponse>();
        CreateMap<Project, ProjectDetailResponse>();
        CreateMap<CreateProjectRequest, Project>();
        CreateMap<UpdateProjectRequest, Project>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TestSuites, opt => opt.Ignore())
            .ForMember(dest => dest.TestRuns, opt => opt.Ignore());

        CreateMap<TestSuite, TestSuiteResponse>();
        CreateMap<TestSuite, TestSuiteDetailResponse>();
        CreateMap<CreateTestSuiteRequest, TestSuite>();
        CreateMap<UpdateTestSuiteRequest, TestSuite>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProjectId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Project, opt => opt.Ignore())
            .ForMember(dest => dest.TestCases, opt => opt.Ignore());

        CreateMap<TestCase, TestCaseResponse>();
        CreateMap<TestCase, TestCaseDetailResponse>();
        CreateMap<CreateTestCaseRequest, TestCase>();
        CreateMap<UpdateTestCaseRequest, TestCase>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SuiteId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Suite, opt => opt.Ignore())
            .ForMember(dest => dest.Steps, opt => opt.Ignore());

        CreateMap<TestCaseStep, TestCaseStepResponse>();
        CreateMap<CreateTestCaseStepRequest, TestCaseStep>();
        CreateMap<UpdateTestCaseStepRequest, TestCaseStep>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TestCaseId, opt => opt.Ignore())
            .ForMember(dest => dest.TestCase, opt => opt.Ignore());

        CreateMap<TestRun, TestRunResponse>();
        CreateMap<CreateTestRunRequest, TestRun>();
        CreateMap<UpdateTestRunRequest, TestRun>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProjectId, opt => opt.Ignore())
            .ForMember(dest => dest.Project, opt => opt.Ignore())
            .ForMember(dest => dest.TestResults, opt => opt.Ignore());

        CreateMap<TestResult, TestResultResponse>();
        CreateMap<CreateTestResultRequest, TestResult>();
        CreateMap<UpdateTestResultRequest, TestResult>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TestRunId, opt => opt.Ignore())
            .ForMember(dest => dest.ExecutedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ExecutedBy, opt => opt.Ignore())
            .ForMember(dest => dest.TestRun, opt => opt.Ignore())
            .ForMember(dest => dest.TestCase, opt => opt.Ignore());
    }
}
