-- Seed initial customer tree
INSERT OR IGNORE INTO customers (Id, Name, Version) VALUES
    ('11111111-2222-3333-4444-555555555555', 'Contoso Ltd', '1.0.0');

INSERT OR IGNORE INTO branches (Id, CustomerId, Name, Version) VALUES
    ('aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee', '11111111-2222-3333-4444-555555555555', 'Headquarters', '1.0.0');

INSERT OR IGNORE INTO nodes (Id, BranchId, Environment, Version, NodeToken) VALUES
    ('99999999-8888-7777-6666-555555555555', 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee', 1, '1.0.0', NULL);
