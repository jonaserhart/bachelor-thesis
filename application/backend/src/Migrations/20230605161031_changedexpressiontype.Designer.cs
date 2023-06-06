﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using backend.Services.Database;

#nullable disable

namespace backend.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230605161031_changedexpressiontype")]
    partial class changedexpressiontype
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("backend.Model.Analysis.AnalysisModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("TeamId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("TeamId");

                    b.ToTable("AnalysisModels");
                });

            modelBuilder.Entity("backend.Model.Analysis.Clause", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Field")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FieldValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsFieldValue")
                        .HasColumnType("boolean");

                    b.Property<int>("LogicalOperator")
                        .HasColumnType("integer");

                    b.Property<Guid?>("ParentClauseId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("QueryId")
                        .HasColumnType("uuid");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ParentClauseId");

                    b.HasIndex("QueryId")
                        .IsUnique();

                    b.ToTable("Clause");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.Expression", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Expressions");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Expression");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("backend.Model.Analysis.FieldInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("QueryId")
                        .HasColumnType("uuid");

                    b.Property<string>("ReferenceName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("QueryId");

                    b.ToTable("FieldInfos");
                });

            modelBuilder.Entity("backend.Model.Analysis.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("backend.Model.Analysis.Query", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ModelId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ReferencedId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ModelId");

                    b.ToTable("Queries");
                });

            modelBuilder.Entity("backend.Model.Analysis.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Team");
                });

            modelBuilder.Entity("backend.Model.Users.RefreshToken", b =>
                {
                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Token");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("backend.Model.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EMail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("backend.Model.Users.UserModel", b =>
                {
                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ModelId")
                        .HasColumnType("uuid");

                    b.Property<string>("Permissions")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId", "ModelId");

                    b.HasIndex("ModelId");

                    b.ToTable("UserModels");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.AvgExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<Guid?>("FieldExpressionId")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("uuid");

                    b.HasIndex("FieldExpressionId");

                    b.HasDiscriminator().HasValue("AvgExpression");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.CountIfExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<string>("Field")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Operator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasDiscriminator().HasValue("CountIfExpression");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.DivExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<Guid?>("LeftId")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("RightId")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("uuid");

                    b.HasIndex("LeftId");

                    b.HasIndex("RightId");

                    b.HasDiscriminator().HasValue("DivExpression");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.FieldExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<string>("Field")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("Expressions", t =>
                        {
                            t.Property("Field")
                                .HasColumnName("FieldExpression_Field");
                        });

                    b.HasDiscriminator().HasValue("FieldExpression");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.MaxExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<Guid?>("FieldExpressionId")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("uuid");

                    b.HasIndex("FieldExpressionId");

                    b.HasDiscriminator().HasValue("MaxExpression");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.MinExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<Guid?>("FieldExpressionId")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("uuid");

                    b.HasIndex("FieldExpressionId");

                    b.HasDiscriminator().HasValue("MinExpression");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.MultiplyExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<Guid?>("LeftId")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("RightId")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("uuid");

                    b.HasIndex("LeftId");

                    b.HasIndex("RightId");

                    b.HasDiscriminator().HasValue("MultiplyExpression");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.NumericValueExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<double?>("Value")
                        .HasColumnType("double precision");

                    b.ToTable("Expressions", t =>
                        {
                            t.Property("Value")
                                .HasColumnName("NumericValueExpression_Value");
                        });

                    b.HasDiscriminator().HasValue("NumericValueExpression");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.SubtractExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<Guid?>("LeftId")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("RightId")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("uuid");

                    b.HasIndex("LeftId");

                    b.HasIndex("RightId");

                    b.HasDiscriminator().HasValue("SubtractExpression");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.SumExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<Guid?>("FieldExpressionId")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("uuid");

                    b.HasIndex("FieldExpressionId");

                    b.HasDiscriminator().HasValue("SumExpression");
                });

            modelBuilder.Entity("backend.Model.Analysis.AnalysisModel", b =>
                {
                    b.HasOne("backend.Model.Analysis.Project", "Project")
                        .WithMany("Models")
                        .HasForeignKey("ProjectId");

                    b.HasOne("backend.Model.Analysis.Team", "Team")
                        .WithMany("Models")
                        .HasForeignKey("TeamId");

                    b.Navigation("Project");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("backend.Model.Analysis.Clause", b =>
                {
                    b.HasOne("backend.Model.Analysis.Clause", "ParentClause")
                        .WithMany("Clauses")
                        .HasForeignKey("ParentClauseId");

                    b.HasOne("backend.Model.Analysis.Query", "Query")
                        .WithOne("Where")
                        .HasForeignKey("backend.Model.Analysis.Clause", "QueryId");

                    b.OwnsOne("backend.Model.Analysis.FieldOperation", "Operator", b1 =>
                        {
                            b1.Property<Guid>("ClauseId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("ReferenceName")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("ClauseId");

                            b1.ToTable("Clause");

                            b1.WithOwner()
                                .HasForeignKey("ClauseId");
                        });

                    b.Navigation("Operator");

                    b.Navigation("ParentClause");

                    b.Navigation("Query");
                });

            modelBuilder.Entity("backend.Model.Analysis.FieldInfo", b =>
                {
                    b.HasOne("backend.Model.Analysis.Query", "Query")
                        .WithMany("Select")
                        .HasForeignKey("QueryId");

                    b.Navigation("Query");
                });

            modelBuilder.Entity("backend.Model.Analysis.Query", b =>
                {
                    b.HasOne("backend.Model.Analysis.AnalysisModel", "Model")
                        .WithMany("Queries")
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.ClientCascade);

                    b.Navigation("Model");
                });

            modelBuilder.Entity("backend.Model.Users.RefreshToken", b =>
                {
                    b.HasOne("backend.Model.Users.User", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientCascade);

                    b.Navigation("User");
                });

            modelBuilder.Entity("backend.Model.Users.UserModel", b =>
                {
                    b.HasOne("backend.Model.Analysis.AnalysisModel", "Model")
                        .WithMany("ModelUsers")
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend.Model.Users.User", "User")
                        .WithMany("UserModels")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Model");

                    b.Navigation("User");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.AvgExpression", b =>
                {
                    b.HasOne("backend.Model.Analysis.Expressions.FieldExpression", "FieldExpression")
                        .WithMany()
                        .HasForeignKey("FieldExpressionId");

                    b.Navigation("FieldExpression");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.DivExpression", b =>
                {
                    b.HasOne("backend.Model.Analysis.Expressions.Expression", "Left")
                        .WithMany()
                        .HasForeignKey("LeftId");

                    b.HasOne("backend.Model.Analysis.Expressions.Expression", "Right")
                        .WithMany()
                        .HasForeignKey("RightId");

                    b.Navigation("Left");

                    b.Navigation("Right");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.MaxExpression", b =>
                {
                    b.HasOne("backend.Model.Analysis.Expressions.FieldExpression", "FieldExpression")
                        .WithMany()
                        .HasForeignKey("FieldExpressionId");

                    b.Navigation("FieldExpression");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.MinExpression", b =>
                {
                    b.HasOne("backend.Model.Analysis.Expressions.FieldExpression", "FieldExpression")
                        .WithMany()
                        .HasForeignKey("FieldExpressionId");

                    b.Navigation("FieldExpression");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.MultiplyExpression", b =>
                {
                    b.HasOne("backend.Model.Analysis.Expressions.Expression", "Left")
                        .WithMany()
                        .HasForeignKey("LeftId");

                    b.HasOne("backend.Model.Analysis.Expressions.Expression", "Right")
                        .WithMany()
                        .HasForeignKey("RightId");

                    b.Navigation("Left");

                    b.Navigation("Right");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.SubtractExpression", b =>
                {
                    b.HasOne("backend.Model.Analysis.Expressions.Expression", "Left")
                        .WithMany()
                        .HasForeignKey("LeftId");

                    b.HasOne("backend.Model.Analysis.Expressions.Expression", "Right")
                        .WithMany()
                        .HasForeignKey("RightId");

                    b.Navigation("Left");

                    b.Navigation("Right");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.SumExpression", b =>
                {
                    b.HasOne("backend.Model.Analysis.Expressions.FieldExpression", "FieldExpression")
                        .WithMany()
                        .HasForeignKey("FieldExpressionId");

                    b.Navigation("FieldExpression");
                });

            modelBuilder.Entity("backend.Model.Analysis.AnalysisModel", b =>
                {
                    b.Navigation("ModelUsers");

                    b.Navigation("Queries");
                });

            modelBuilder.Entity("backend.Model.Analysis.Clause", b =>
                {
                    b.Navigation("Clauses");
                });

            modelBuilder.Entity("backend.Model.Analysis.Project", b =>
                {
                    b.Navigation("Models");
                });

            modelBuilder.Entity("backend.Model.Analysis.Query", b =>
                {
                    b.Navigation("Select");

                    b.Navigation("Where");
                });

            modelBuilder.Entity("backend.Model.Analysis.Team", b =>
                {
                    b.Navigation("Models");
                });

            modelBuilder.Entity("backend.Model.Users.User", b =>
                {
                    b.Navigation("RefreshTokens");

                    b.Navigation("UserModels");
                });
#pragma warning restore 612, 618
        }
    }
}
