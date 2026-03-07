using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterTests.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateInitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "test_runs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    environment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    executed_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_runs", x => x.id);
                    table.ForeignKey(
                        name: "fk_test_runs_projects",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "test_suites",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_suites", x => x.id);
                    table.ForeignKey(
                        name: "fk_test_suites_projects",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "test_cases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    suite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    preconditions = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    postconditions = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_cases", x => x.id);
                    table.ForeignKey(
                        name: "fk_test_cases_test_suites",
                        column: x => x.suite_id,
                        principalTable: "test_suites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "test_case_steps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    test_case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_order = table.Column<int>(type: "integer", nullable: false),
                    action = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    expected_result = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_case_steps", x => x.id);
                    table.ForeignKey(
                        name: "fk_test_case_steps_test_cases",
                        column: x => x.test_case_id,
                        principalTable: "test_cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "test_results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    test_run_id = table.Column<Guid>(type: "uuid", nullable: false),
                    test_case_id = table.Column<Guid>(type: "uuid", nullable: true),
                    result = table.Column<int>(type: "integer", nullable: false),
                    comments = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    defect_link = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    executed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    executed_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_results", x => x.id);
                    table.ForeignKey(
                        name: "fk_test_results_test_cases",
                        column: x => x.test_case_id,
                        principalTable: "test_cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_test_results_test_runs",
                        column: x => x.test_run_id,
                        principalTable: "test_runs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_projects_name_unique",
                table: "projects",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_test_case_steps_case_id",
                table: "test_case_steps",
                column: "test_case_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_case_steps_case_id_order_unique",
                table: "test_case_steps",
                columns: new[] { "test_case_id", "step_order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_test_cases_priority",
                table: "test_cases",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "ix_test_cases_status",
                table: "test_cases",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_test_cases_suite_id_name_unique",
                table: "test_cases",
                columns: new[] { "suite_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_test_results_result",
                table: "test_results",
                column: "result");

            migrationBuilder.CreateIndex(
                name: "ix_test_results_run_id_case_id_unique",
                table: "test_results",
                columns: new[] { "test_run_id", "test_case_id" },
                unique: true,
                filter: "\"test_case_id\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_test_results_test_case_id",
                table: "test_results",
                column: "test_case_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_results_test_run_id",
                table: "test_results",
                column: "test_run_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_runs_project_id",
                table: "test_runs",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_runs_started_at",
                table: "test_runs",
                column: "started_at");

            migrationBuilder.CreateIndex(
                name: "ix_test_runs_status",
                table: "test_runs",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_test_suites_project_id_name_unique",
                table: "test_suites",
                columns: new[] { "project_id", "name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "test_case_steps");

            migrationBuilder.DropTable(
                name: "test_results");

            migrationBuilder.DropTable(
                name: "test_cases");

            migrationBuilder.DropTable(
                name: "test_runs");

            migrationBuilder.DropTable(
                name: "test_suites");

            migrationBuilder.DropTable(
                name: "projects");
        }
    }
}
