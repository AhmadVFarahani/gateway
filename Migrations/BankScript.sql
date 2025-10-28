IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Companies] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(30) NOT NULL,
    [Description] nvarchar(200) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY ([Id])
);

CREATE TABLE [Plans] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(60) NOT NULL,
    [MonthlyPrice] decimal(18,2) NOT NULL,
    [MaxRequestsPerMonth] int NOT NULL,
    [IsUnlimited] bit NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Plans] PRIMARY KEY ([Id])
);

CREATE TABLE [Roles] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(30) NOT NULL,
    [Description] nvarchar(200) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

CREATE TABLE [Scopes] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [DisplayName] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Scopes] PRIMARY KEY ([Id])
);

CREATE TABLE [Services] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [BaseUrl] nvarchar(50) NOT NULL,
    [IsActive] bit NOT NULL,
    [Description] nvarchar(100) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Services] PRIMARY KEY ([Id])
);

CREATE TABLE [CompanyPlans] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [PlanId] bigint NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NULL,
    [AutoRenew] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_CompanyPlans] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CompanyPlans_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CompanyPlans_Plans_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [Plans] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Users] (
    [Id] bigint NOT NULL IDENTITY,
    [UserName] nvarchar(30) NOT NULL,
    [PasswordHash] nvarchar(200) NOT NULL,
    [IsActive] bit NOT NULL,
    [UserType] smallint NOT NULL,
    [CompanyId] bigint NOT NULL,
    [RoleId] bigint NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Routes] (
    [Id] bigint NOT NULL IDENTITY,
    [Path] nvarchar(70) NOT NULL,
    [HttpMethod] smallint NOT NULL,
    [TargetPath] nvarchar(50) NOT NULL,
    [ServiceId] bigint NOT NULL,
    [RequiresAuthentication] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Routes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Routes_Services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ApiKeys] (
    [Id] bigint NOT NULL IDENTITY,
    [Key] nvarchar(200) NOT NULL,
    [UserId] bigint NOT NULL,
    [IsActive] bit NOT NULL,
    [ExpirationDate] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_ApiKeys] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ApiKeys_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [CompanyRoutePricing] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [RouteId] bigint NOT NULL,
    [PricePerCall] decimal(18,2) NULL,
    [MaxFreeCallsPerMonth] int NULL,
    [BillingType] nvarchar(max) NOT NULL,
    [MaxFreeCalls] int NULL,
    [TieredJson] nvarchar(max) NULL,
    [MonthlySubscriptionPrice] decimal(18,2) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_CompanyRoutePricing] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CompanyRoutePricing_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CompanyRoutePricing_Routes_RouteId] FOREIGN KEY ([RouteId]) REFERENCES [Routes] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [PlanRoutes] (
    [Id] bigint NOT NULL IDENTITY,
    [PlanId] bigint NOT NULL,
    [RouteId] bigint NOT NULL,
    [FlatPricePerCall] decimal(18,2) NULL,
    [TieredPricingJson] nvarchar(200) NULL,
    [IsFree] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_PlanRoutes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PlanRoutes_Plans_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [Plans] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PlanRoutes_Routes_RouteId] FOREIGN KEY ([RouteId]) REFERENCES [Routes] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [RouteRequestFields] (
    [Id] bigint NOT NULL IDENTITY,
    [RouteId] bigint NOT NULL,
    [FieldName] nvarchar(200) NOT NULL,
    [Type] nvarchar(100) NOT NULL,
    [Format] nvarchar(100) NULL,
    [IsRequired] bit NOT NULL,
    [Description] nvarchar(500) NULL,
    [ParentId] bigint NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_RouteRequestFields] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RouteRequestFields_RouteRequestFields_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [RouteRequestFields] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RouteRequestFields_Routes_RouteId] FOREIGN KEY ([RouteId]) REFERENCES [Routes] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [RouteResponseFields] (
    [Id] bigint NOT NULL IDENTITY,
    [RouteId] bigint NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Type] nvarchar(100) NULL,
    [Description] nvarchar(1000) NULL,
    [IsRequired] bit NOT NULL,
    [ParentId] bigint NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_RouteResponseFields] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RouteResponseFields_RouteResponseFields_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [RouteResponseFields] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RouteResponseFields_Routes_RouteId] FOREIGN KEY ([RouteId]) REFERENCES [Routes] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [RouteScopes] (
    [Id] bigint NOT NULL IDENTITY,
    [RouteId] bigint NOT NULL,
    [ScopeId] bigint NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_RouteScopes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RouteScopes_Routes_RouteId] FOREIGN KEY ([RouteId]) REFERENCES [Routes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RouteScopes_Scopes_ScopeId] FOREIGN KEY ([ScopeId]) REFERENCES [Scopes] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AccessPolicies] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NULL,
    [ApiKeyId] bigint NULL,
    [ScopeId] bigint NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_AccessPolicies] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AccessPolicies_ApiKeys_ApiKeyId] FOREIGN KEY ([ApiKeyId]) REFERENCES [ApiKeys] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AccessPolicies_Scopes_ScopeId] FOREIGN KEY ([ScopeId]) REFERENCES [Scopes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AccessPolicies_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [RefreshTokens] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NULL,
    [ApiKeyId] bigint NULL,
    [Token] nvarchar(500) NOT NULL,
    [ExpirationDate] datetime2 NOT NULL,
    [IsRevoked] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RefreshTokens_ApiKeys_ApiKeyId] FOREIGN KEY ([ApiKeyId]) REFERENCES [ApiKeys] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_AccessPolicies_ApiKeyId] ON [AccessPolicies] ([ApiKeyId]);

CREATE INDEX [IX_AccessPolicies_ScopeId] ON [AccessPolicies] ([ScopeId]);

CREATE INDEX [IX_AccessPolicies_UserId] ON [AccessPolicies] ([UserId]);

CREATE UNIQUE INDEX [IX_ApiKeys_Key] ON [ApiKeys] ([Key]);

CREATE INDEX [IX_ApiKeys_UserId] ON [ApiKeys] ([UserId]);

CREATE INDEX [IX_CompanyPlans_CompanyId] ON [CompanyPlans] ([CompanyId]);

CREATE INDEX [IX_CompanyPlans_PlanId] ON [CompanyPlans] ([PlanId]);

CREATE INDEX [IX_CompanyRoutePricing_CompanyId] ON [CompanyRoutePricing] ([CompanyId]);

CREATE INDEX [IX_CompanyRoutePricing_RouteId] ON [CompanyRoutePricing] ([RouteId]);

CREATE INDEX [IX_PlanRoutes_PlanId] ON [PlanRoutes] ([PlanId]);

CREATE INDEX [IX_PlanRoutes_RouteId] ON [PlanRoutes] ([RouteId]);

CREATE INDEX [IX_RefreshTokens_ApiKeyId] ON [RefreshTokens] ([ApiKeyId]);

CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);

CREATE INDEX [IX_RouteRequestFields_ParentId] ON [RouteRequestFields] ([ParentId]);

CREATE INDEX [IX_RouteRequestFields_RouteId] ON [RouteRequestFields] ([RouteId]);

CREATE INDEX [IX_RouteResponseFields_ParentId] ON [RouteResponseFields] ([ParentId]);

CREATE INDEX [IX_RouteResponseFields_RouteId] ON [RouteResponseFields] ([RouteId]);

CREATE INDEX [IX_Routes_ServiceId] ON [Routes] ([ServiceId]);

CREATE INDEX [IX_RouteScopes_RouteId] ON [RouteScopes] ([RouteId]);

CREATE INDEX [IX_RouteScopes_ScopeId] ON [RouteScopes] ([ScopeId]);

CREATE UNIQUE INDEX [IX_Scopes_Name] ON [Scopes] ([Name]);

CREATE INDEX [IX_Users_CompanyId] ON [Users] ([CompanyId]);

CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250706071719_db1', N'9.0.4');

CREATE TABLE [Invoices] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [ContractId] bigint NOT NULL,
    [PeriodFrom] datetime2 NOT NULL,
    [PeriodTo] datetime2 NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Invoices] PRIMARY KEY ([Id])
);

CREATE TABLE [InvoiceItems] (
    [Id] bigint NOT NULL IDENTITY,
    [InvoiceId] bigint NOT NULL,
    [RouteId] uniqueidentifier NULL,
    [UsageCount] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [SubTotal] decimal(18,2) NOT NULL,
    [TierDetails] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_InvoiceItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_InvoiceItems_Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_InvoiceItems_InvoiceId] ON [InvoiceItems] ([InvoiceId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250711221847_invoice', N'9.0.4');

DROP TABLE [CompanyRoutePricing];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250711232759_contract', N'9.0.4');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250711233017_contracts', N'9.0.4');

ALTER TABLE [CompanyPlans] DROP CONSTRAINT [FK_CompanyPlans_Companies_CompanyId];

ALTER TABLE [CompanyPlans] DROP CONSTRAINT [FK_CompanyPlans_Plans_PlanId];

ALTER TABLE [CompanyPlans] DROP CONSTRAINT [PK_CompanyPlans];

EXEC sp_rename N'[CompanyPlans]', N'Contracts', 'OBJECT';

EXEC sp_rename N'[Contracts].[IX_CompanyPlans_PlanId]', N'IX_Contracts_PlanId', 'INDEX';

EXEC sp_rename N'[Contracts].[IX_CompanyPlans_CompanyId]', N'IX_Contracts_CompanyId', 'INDEX';

ALTER TABLE [Contracts] ADD CONSTRAINT [PK_Contracts] PRIMARY KEY ([Id]);

ALTER TABLE [Contracts] ADD CONSTRAINT [FK_Contracts_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE;

ALTER TABLE [Contracts] ADD CONSTRAINT [FK_Contracts_Plans_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [Plans] ([Id]) ON DELETE CASCADE;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250712031817_contract-tabel', N'9.0.4');


        ALTER TABLE InvoiceItems DROP COLUMN RouteId;
        ALTER TABLE InvoiceItems ADD RouteId bigint NULL;
    

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250712055818_edit-invoice-item', N'9.0.4');

CREATE TABLE [UsageLogs] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [ContractId] bigint NOT NULL,
    [PlanId] bigint NOT NULL,
    [RouteId] bigint NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [ResponseStatusCode] int NOT NULL,
    [DurationMs] bigint NOT NULL,
    [IsBilled] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_UsageLogs] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250714044559_usage-log', N'9.0.4');

ALTER TABLE [Plans] ADD [PricingType] int NOT NULL DEFAULT 0;

ALTER TABLE [Plans] ADD [RequestPrice] decimal(18,2) NOT NULL DEFAULT 0.0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250802231623_pricing-type', N'9.0.4');

ALTER TABLE [Contracts] ADD [Description] nvarchar(100) NOT NULL DEFAULT N'-';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250810030722_contract-description', N'9.0.4');

DECLARE @var sysname;
SELECT @var = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UsageLogs]') AND [c].[name] = N'Timestamp');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [UsageLogs] DROP CONSTRAINT [' + @var + '];');
ALTER TABLE [UsageLogs] ALTER COLUMN [Timestamp] datetime2(3) NOT NULL;
ALTER TABLE [UsageLogs] ADD DEFAULT (GETUTCDATE()) FOR [Timestamp];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251022044310_UPDATE-USAGE-LOG', N'9.0.4');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251022044705_UPDATE-INVOICE', N'9.0.4');

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[InvoiceItems]') AND [c].[name] = N'TierDetails');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [InvoiceItems] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [InvoiceItems] ALTER COLUMN [TierDetails] nvarchar(200) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251022044935_UPDATE-INVOICE-decimal', N'9.0.4');

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UsageLogs]') AND [c].[name] = N'Timestamp');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [UsageLogs] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [UsageLogs] DROP COLUMN [Timestamp];

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UsageLogs]') AND [c].[name] = N'CreatedAt');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [UsageLogs] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [UsageLogs] ALTER COLUMN [CreatedAt] datetime2(3) NOT NULL;
ALTER TABLE [UsageLogs] ADD DEFAULT (GETUTCDATE()) FOR [CreatedAt];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251022190101_usage-log-fix', N'9.0.4');

ALTER TABLE [UsageLogs] ADD [UserId] bigint NOT NULL DEFAULT CAST(0 AS bigint);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251022191927_usage-log-userid', N'9.0.4');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251022192013_usage-log-keyId', N'9.0.4');

ALTER TABLE [UsageLogs] ADD [KeyId] bigint NOT NULL DEFAULT CAST(0 AS bigint);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251022192121_usage-log-keyIdlog', N'9.0.4');

COMMIT;
GO

