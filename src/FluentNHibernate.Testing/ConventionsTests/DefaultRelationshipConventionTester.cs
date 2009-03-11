using FluentNHibernate.Mapping;
using FluentNHibernate.Mapping.Conventions;
using NUnit.Framework;
using Rhino.Mocks;

namespace FluentNHibernate.Testing.ConventionsTests
{
    [TestFixture]
    public class DefaultRelationshipConventionTester
    {
        private DefaultRelationshipConvention convention;
        private IConventionFinder conventionFinder;
        private readonly ConventionOverrides Overrides = new ConventionOverrides();

        [SetUp]
        public void CreateConvention()
        {
            conventionFinder = MockRepository.GenerateMock<IConventionFinder>();
            convention = new DefaultRelationshipConvention(conventionFinder);
        }

        [Test]
        public void ShouldAcceptIRelationsips()
        {
            convention.Accept(MockRepository.GenerateStub<IRelationship>())
                .ShouldBeTrue();
        }

        [Test]
        public void ShouldntAcceptAnyOtherIMappingParts()
        {
            convention.Accept(MockRepository.GenerateStub<IProperty>())
                .ShouldBeFalse();
        }

        [Test]
        public void ApplyFindsConventions()
        {
            conventionFinder.Stub(x => x.Find<IRelationshipConvention>())
                .Return(new IRelationshipConvention[] { });

            convention.Apply(MockRepository.GenerateStub<IRelationship>(), Overrides);
            conventionFinder.AssertWasCalled(x => x.Find<IRelationshipConvention>());
        }

        [Test]
        public void ApplyCallsAcceptOnAllConventionsAgainstEachClass()
        {
            var conventions = new[]
            {
                MockRepository.GenerateMock<IRelationshipConvention>(),
                MockRepository.GenerateMock<IRelationshipConvention>()
            };
            var relationship = MockRepository.GenerateStub<IRelationship>();

            conventionFinder.Stub(x => x.Find<IRelationshipConvention>())
                .Return(conventions);

            convention.Apply(relationship, Overrides);

            conventions[0].AssertWasCalled(x => x.Accept(relationship));
            conventions[1].AssertWasCalled(x => x.Accept(relationship));
        }

        [Test]
        public void ApplyAppliesAllAcceptedConventions()
        {
            var conventions = new[]
            {
                MockRepository.GenerateMock<IRelationshipConvention>(),
                MockRepository.GenerateMock<IRelationshipConvention>()
            };
            var relationship = MockRepository.GenerateStub<IRelationship>();

            conventionFinder.Stub(x => x.Find<IRelationshipConvention>())
                .Return(conventions);

            conventions[0].Stub(x => x.Accept(relationship)).Return(true);
            conventions[1].Stub(x => x.Accept(relationship)).Return(false);

            convention.Apply(relationship, Overrides);

            // each convention gets Apply called for any properties it returned true for Accept
            conventions[0].AssertWasCalled(x => x.Apply(relationship, Overrides));
            conventions[1].AssertWasNotCalled(x => x.Apply(relationship, Overrides));
        }
    }
}