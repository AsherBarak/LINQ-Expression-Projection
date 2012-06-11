using System;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using LinqExpressionProjection.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqExpressionProjection.Test
{
    [TestClass]
    public class LinqExpressionsProjection_TestFixture
    {
        private static Expression<Func<Project, double>> _projectAverageEffectiveAreaSelectorStatic =
                proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);

        private Expression<Func<Project, double>> _projectAverageEffectiveAreaSelector =
        proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);

        public static Expression<Func<Project, double>> GetProjectAverageEffectiveAreaSelectorStatic()
        {
            return proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);
        }

        public Expression<Func<Project, double>> GetProjectAverageEffectiveAreaSelector()
        {
            return proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);
        }

        public Expression<Func<Project, double>> GetProjectAverageEffectiveAreaSelectorWithLogic(bool isOverThousandIncluded = false)
        {
            return isOverThousandIncluded
            ? (Expression<Func<Project, double>>) ((Project proj) => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area))
            : (Project proj) => proj.Subprojects.Average(sp => sp.Area);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ProjectingExpressionFailsOnNormalCases_Test()
        {
            ValidateDb();
            Expression<Func<Project, double>> localSelector =
                proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);
            using (var ctx = new ProjectsDbContext())
            {
                var v = (from p in ctx.Projects
                         select new
                         {
                             Project = p,
                             AEA = localSelector
                         }).ToArray();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ProjectingExpressionFailsWithNoCallToAsExpressionProjectable_Test()
        {
            ValidateDb();
            Expression<Func<Project, double>> localSelector =
                proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects
                                select new
                                {
                                    Project = p,
                                    AEA = localSelector.Project<double>()
                                }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ProjectingExpressionFailsWithProjectionNotMatchingLambdaReturnType_Test()
        {
            ValidateDb();
            Expression<Func<Project, double>> localSelector =
                proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                {
                                    Project = p,
                                    AEA = localSelector.Project<int>()
                                }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ProjectingExpressionFailsWithWrongLambdaParameterType_Test()
        {
            ValidateDb();
            Expression<Func<Subproject, double>> localSelector =
                sp => sp.Area;
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                {
                                    Project = p,
                                    AEA = localSelector.Project<double>()
                                }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }

        [TestMethod]
        public void ProjectingExpressionByLocalVariable_Test()
        {
            ValidateDb();
            Expression<Func<Project, double>> localSelector =
                proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                           {
                                               Project = p,
                                               AEA = localSelector.Project<double>()
                                           }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }

        [TestMethod]
        public void ProjectingExpressionByStaticField_Test()
        {
            ValidateDb();
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                           {
                                               Project = p,
                                               AEA = _projectAverageEffectiveAreaSelectorStatic.Project<double>()
                                           }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }


        [TestMethod]
        public void ProjectingExpressionByNonStaticField_Test()
        {
            ValidateDb();
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                {
                                    Project = p,
                                    AEA = _projectAverageEffectiveAreaSelector.Project<double>()
                                }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }

        [TestMethod]
        public void ProjectingExpressionByStaticMethod_Test()
        {
            ValidateDb();
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                {
                                    Project = p,
                                    AEA = GetProjectAverageEffectiveAreaSelectorStatic().Project<double>()
                                }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }

        [TestMethod]
        public void ProjectingExpressionByNonStaticMethod_Test()
        {
            ValidateDb();
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                {
                                    Project = p,
                                    AEA = GetProjectAverageEffectiveAreaSelector().Project<double>()
                                }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }

        [TestMethod]
        public void ProjectingExpressionByNonStaticMethodWithLogic_Test()
        {
            ValidateDb();
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                {
                                    Project = p,
                                    AEA = GetProjectAverageEffectiveAreaSelectorWithLogic(false).Project<double>()
                                }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(3600, projects[1].AEA);
            }
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                {
                                    Project = p,
                                    AEA = GetProjectAverageEffectiveAreaSelectorWithLogic(true).Project<double>()
                                }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }

        private static void ValidateDb()
        {
            using (var ctx = new ProjectsDbContext())
            {
                try
                {
                    if (ctx.Projects.Count() != 2 || ctx.Subprojects.Count() != 5)
                    {
                        ClearDb();
                        PopulateDb();
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        ClearDb();
                    }
                    catch (Exception)
                    {
                    }
                    PopulateDb();
                }
            }
        }

        private static void ClearDb()
        {
            using (var ctx = new ProjectsDbContext())
            {
                foreach (var subproject in ctx.Subprojects)
                {
                    ctx.Subprojects.Remove(subproject);
                }
                foreach (var project in ctx.Projects)
                {
                    ctx.Projects.Remove(project);
                }
                ctx.SaveChanges();
            }
        }

        private static void PopulateDb()
        {
            using (var ctx = new ProjectsDbContext())
            {
                Project p1 = ctx.Projects.Add(new Project());
                Project p2 = ctx.Projects.Add(new Project());

                ctx.Subprojects.Add(new Subproject() { Area = 100, Project = p1 });
                ctx.Subprojects.Add(new Subproject() { Area = 200, Project = p1 });
                ctx.Subprojects.Add(new Subproject() { Area = 350, Project = p2 });
                ctx.Subprojects.Add(new Subproject() { Area = 450, Project = p2 });
                ctx.Subprojects.Add(new Subproject() { Area = 10000, Project = p2 });

                ctx.SaveChanges();
            }
        }
    }
}
