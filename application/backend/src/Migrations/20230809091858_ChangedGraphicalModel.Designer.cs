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
    [Migration("20230809091858_ChangedGraphicalModel")]
    partial class ChangedGraphicalModel
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

                    b.HasKey("Id");

                    b.ToTable("AnalysisModels");
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.Expression", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("QueryId")
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Expressions");

                    b.HasDiscriminator<int>("Type");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("backend.Model.Analysis.Graphical.GraphicalConfiguration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ModelId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ModelId");

                    b.ToTable("GraphicalConfigurations");
                });

            modelBuilder.Entity("backend.Model.Analysis.Graphical.GraphicalItemDataSources", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ItemId")
                        .HasColumnType("uuid");

                    b.Property<string>("KPIs")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.HasIndex("ItemId")
                        .IsUnique();

                    b.ToTable("GraphicalItemDataSources");
                });

            modelBuilder.Entity("backend.Model.Analysis.Graphical.GraphicalReportItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("GraphicalConfigId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("GraphicalConfigId");

                    b.ToTable("GraphicalReportItems");
                });

            modelBuilder.Entity("backend.Model.Analysis.Graphical.GraphicalReportItemLayout", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("H")
                        .HasColumnType("integer");

                    b.Property<Guid>("I")
                        .HasColumnType("uuid");

                    b.Property<int>("W")
                        .HasColumnType("integer");

                    b.Property<int>("X")
                        .HasColumnType("integer");

                    b.Property<int>("Y")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("I")
                        .IsUnique();

                    b.ToTable("GraphicalReportItemLayout");
                });

            modelBuilder.Entity("backend.Model.Analysis.KPI", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AcceptableValues")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("any");

                    b.Property<Guid?>("AnalysisModelId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ExpressionId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("ShowInReport")
                        .HasColumnType("boolean");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AnalysisModelId");

                    b.HasIndex("ExpressionId");

                    b.ToTable("KPIs");
                });

            modelBuilder.Entity("backend.Model.Analysis.Report", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AnalysisModelId")
                        .HasColumnType("uuid");

                    b.Property<long>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasDefaultValueSql("EXTRACT(EPOCH FROM NOW())::BIGINT");

                    b.Property<string>("KPIsAndValues")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("QueryResults")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AnalysisModelId");

                    b.ToTable("Reports");
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

                    b.Property<string>("Field")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("Expressions", t =>
                        {
                            t.Property("Field")
                                .HasColumnName("AvgExpression_Field");
                        });

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.CountExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.HasDiscriminator().HasValue(10);
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.CountIfExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<string>("CompareValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Field")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Operator")
                        .HasColumnType("integer");

                    b.HasDiscriminator().HasValue(9);
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.MathOperationExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<Guid?>("LeftId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("RightId")
                        .HasColumnType("uuid");

                    b.HasIndex("LeftId")
                        .IsUnique();

                    b.HasIndex("RightId")
                        .IsUnique();
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.MaxExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<string>("Field")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("Expressions", t =>
                        {
                            t.Property("Field")
                                .HasColumnName("MaxExpression_Field");
                        });

                    b.HasDiscriminator().HasValue(4);
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.MinExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<string>("Field")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("Expressions", t =>
                        {
                            t.Property("Field")
                                .HasColumnName("MinExpression_Field");
                        });

                    b.HasDiscriminator().HasValue(3);
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.NumericValueExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<double?>("Value")
                        .HasColumnType("double precision");

                    b.HasDiscriminator().HasValue(8);
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.PlainQueryExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.HasDiscriminator().HasValue(11);
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.SumExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.Expression");

                    b.Property<string>("Field")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("Expressions", t =>
                        {
                            t.Property("Field")
                                .HasColumnName("SumExpression_Field");
                        });

                    b.HasDiscriminator().HasValue(7);
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.AddExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.MathOperationExpression");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.DivExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.MathOperationExpression");

                    b.HasDiscriminator().HasValue(2);
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.MultiplyExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.MathOperationExpression");

                    b.HasDiscriminator().HasValue(5);
                });

            modelBuilder.Entity("backend.Model.Analysis.Expressions.SubtractExpression", b =>
                {
                    b.HasBaseType("backend.Model.Analysis.Expressions.MathOperationExpression");

                    b.HasDiscriminator().HasValue(6);
                });

            modelBuilder.Entity("backend.Model.Analysis.Graphical.GraphicalConfiguration", b =>
                {
                    b.HasOne("backend.Model.Analysis.AnalysisModel", "Model")
                        .WithMany("Graphical")
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Model");
                });

            modelBuilder.Entity("backend.Model.Analysis.Graphical.GraphicalItemDataSources", b =>
                {
                    b.HasOne("backend.Model.Analysis.Graphical.GraphicalReportItem", "GraphicalReportItem")
                        .WithOne("DataSources")
                        .HasForeignKey("backend.Model.Analysis.Graphical.GraphicalItemDataSources", "ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GraphicalReportItem");
                });

            modelBuilder.Entity("backend.Model.Analysis.Graphical.GraphicalReportItem", b =>
                {
                    b.HasOne("backend.Model.Analysis.Graphical.GraphicalConfiguration", "Configuration")
                        .WithMany("Items")
                        .HasForeignKey("GraphicalConfigId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Configuration");
                });

            modelBuilder.Entity("backend.Model.Analysis.Graphical.GraphicalReportItemLayout", b =>
                {
                    b.HasOne("backend.Model.Analysis.Graphical.GraphicalReportItem", "GraphicalReportItem")
                        .WithOne("Layout")
                        .HasForeignKey("backend.Model.Analysis.Graphical.GraphicalReportItemLayout", "I")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GraphicalReportItem");
                });

            modelBuilder.Entity("backend.Model.Analysis.KPI", b =>
                {
                    b.HasOne("backend.Model.Analysis.AnalysisModel", "AnalysisModel")
                        .WithMany("KPIs")
                        .HasForeignKey("AnalysisModelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("backend.Model.Analysis.Expressions.Expression", "Expression")
                        .WithMany()
                        .HasForeignKey("ExpressionId");

                    b.Navigation("AnalysisModel");

                    b.Navigation("Expression");
                });

            modelBuilder.Entity("backend.Model.Analysis.Report", b =>
                {
                    b.HasOne("backend.Model.Analysis.AnalysisModel", "AnalysisModel")
                        .WithMany("Reports")
                        .HasForeignKey("AnalysisModelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("AnalysisModel");
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

            modelBuilder.Entity("backend.Model.Analysis.Expressions.MathOperationExpression", b =>
                {
                    b.HasOne("backend.Model.Analysis.KPI", "Left")
                        .WithOne()
                        .HasForeignKey("backend.Model.Analysis.Expressions.MathOperationExpression", "LeftId");

                    b.HasOne("backend.Model.Analysis.KPI", "Right")
                        .WithOne()
                        .HasForeignKey("backend.Model.Analysis.Expressions.MathOperationExpression", "RightId");

                    b.Navigation("Left");

                    b.Navigation("Right");
                });

            modelBuilder.Entity("backend.Model.Analysis.AnalysisModel", b =>
                {
                    b.Navigation("Graphical");

                    b.Navigation("KPIs");

                    b.Navigation("ModelUsers");

                    b.Navigation("Reports");
                });

            modelBuilder.Entity("backend.Model.Analysis.Graphical.GraphicalConfiguration", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("backend.Model.Analysis.Graphical.GraphicalReportItem", b =>
                {
                    b.Navigation("DataSources");

                    b.Navigation("Layout");
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
