-- Customers table
CREATE TABLE IF NOT EXISTS customers (
    Id CHAR(36) PRIMARY KEY,
    Name VARCHAR(128) NOT NULL,
    Version VARCHAR(32) NOT NULL UNIQUE
);

-- Branches table
CREATE TABLE IF NOT EXISTS branches (
    Id CHAR(36) PRIMARY KEY,
    CustomerId CHAR(36) NOT NULL,
    Name VARCHAR(128) NOT NULL,
    Version VARCHAR(32) NOT NULL,
    CONSTRAINT fk_branches_customer FOREIGN KEY (CustomerId) REFERENCES customers(Id),
    CONSTRAINT uq_branch_version UNIQUE (CustomerId, Version)
);

-- Nodes table
CREATE TABLE IF NOT EXISTS nodes (
    Id CHAR(36) PRIMARY KEY,
    BranchId CHAR(36) NOT NULL,
    Environment INT NOT NULL,
    Version VARCHAR(32) NOT NULL,
    NodeToken VARCHAR(128) NULL,
    CONSTRAINT fk_nodes_branch FOREIGN KEY (BranchId) REFERENCES branches(Id),
    CONSTRAINT uq_node_version UNIQUE (BranchId, Version),
    CONSTRAINT ck_nodes_environment CHECK (Environment IN (1, 2, 3, 4))
);

-- Node configuration index table (read-only)
CREATE TABLE IF NOT EXISTS node_config_index (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    NodeId CHAR(36) NOT NULL,
    ConfigVersion VARCHAR(32) NOT NULL,
    IsLocked BOOLEAN NOT NULL DEFAULT 0,
    CONSTRAINT fk_index_node FOREIGN KEY (NodeId) REFERENCES nodes(Id)
);
