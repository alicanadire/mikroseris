-- ToyStore PostgreSQL Initialization
-- Inventory Service için tablo oluşturma

-- Inventory tablosu
CREATE TABLE IF NOT EXISTS inventory (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id UUID NOT NULL,
    quantity INTEGER NOT NULL DEFAULT 0,
    reserved_quantity INTEGER NOT NULL DEFAULT 0,
    warehouse_location VARCHAR(100),
    last_updated TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Index'ler
CREATE INDEX IF NOT EXISTS idx_inventory_product_id ON inventory(product_id);
CREATE INDEX IF NOT EXISTS idx_inventory_quantity ON inventory(quantity);

-- Stock movements tablosu
CREATE TABLE IF NOT EXISTS stock_movements (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id UUID NOT NULL,
    movement_type VARCHAR(20) NOT NULL, -- 'IN', 'OUT', 'RESERVED', 'RELEASED'
    quantity INTEGER NOT NULL,
    reason VARCHAR(200),
    reference_id UUID, -- Order ID veya başka referans
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Index'ler
CREATE INDEX IF NOT EXISTS idx_stock_movements_product_id ON stock_movements(product_id);
CREATE INDEX IF NOT EXISTS idx_stock_movements_created_at ON stock_movements(created_at);

-- Sample data insert
INSERT INTO inventory (product_id, quantity, warehouse_location) VALUES 
(gen_random_uuid(), 100, 'A-01-001'),
(gen_random_uuid(), 50, 'A-01-002'),
(gen_random_uuid(), 75, 'B-02-001'),
(gen_random_uuid(), 200, 'B-02-002'),
(gen_random_uuid(), 30, 'C-03-001')
ON CONFLICT DO NOTHING;

PRINT 'PostgreSQL inventory database initialized successfully!';
