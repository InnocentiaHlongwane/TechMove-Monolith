CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "Clients" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Clients" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "ContactDetails" TEXT NOT NULL,
    "Region" TEXT NOT NULL
);

CREATE TABLE "Contracts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Contracts" PRIMARY KEY AUTOINCREMENT,
    "ClientId" INTEGER NOT NULL,
    "StartDate" TEXT NOT NULL,
    "EndDate" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "ServiceLevel" TEXT NOT NULL,
    "SignedAgreementPath" TEXT NOT NULL,
    CONSTRAINT "FK_Contracts_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "ServiceRequests" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ServiceRequests" PRIMARY KEY AUTOINCREMENT,
    "ContractId" INTEGER NOT NULL,
    "Description" TEXT NOT NULL,
    "Cost" TEXT NOT NULL,
    "Currency" TEXT NOT NULL,
    "OriginalUsdAmount" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_ServiceRequests_Contracts_ContractId" FOREIGN KEY ("ContractId") REFERENCES "Contracts" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_Contracts_ClientId" ON "Contracts" ("ClientId");

CREATE INDEX "IX_ServiceRequests_ContractId" ON "ServiceRequests" ("ContractId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261012212643_InitialCreate', '8.0.11');

COMMIT;

