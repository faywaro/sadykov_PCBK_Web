-- ============================================================
-- База данных: sadykov_pcbkdb
-- Схема: app
-- Проект: sadykov_PCBK_Web
-- Пользователь: app / 123456789
-- ============================================================

-- ШАГ 1: Выполнить от суперпользователя postgres
-- ============================================================

DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'app') THEN
        CREATE ROLE app WITH LOGIN PASSWORD '123456789';
    END IF;
END
$$;

-- Создать базу данных (если не существует):
-- CREATE DATABASE sadykov_pcbkdb OWNER app;

-- ШАГ 2: Подключиться к sadykov_pcbkdb (\c sadykov_pcbkdb)
-- ============================================================

-- Схема app
CREATE SCHEMA IF NOT EXISTS app AUTHORIZATION app;
SET search_path TO app;

-- ============================================================
-- Удаление таблиц (для повторного запуска)
-- ============================================================
DROP TABLE IF EXISTS app.partner_sales  CASCADE;
DROP TABLE IF EXISTS app.products       CASCADE;
DROP TABLE IF EXISTS app.partners       CASCADE;
DROP TABLE IF EXISTS app.partner_types  CASCADE;

-- ============================================================
-- Таблица типов партнёров
-- ============================================================
CREATE TABLE app.partner_types (
    id          SERIAL          PRIMARY KEY,
    type_name   VARCHAR(100)    NOT NULL UNIQUE
);

-- ============================================================
-- Таблица партнёров
-- ============================================================
CREATE TABLE app.partners (
    id              SERIAL          PRIMARY KEY,
    type_id         INTEGER         NOT NULL
                        REFERENCES app.partner_types(id) ON DELETE RESTRICT,
    company_name    VARCHAR(255)    NOT NULL,
    legal_address   VARCHAR(500)    NOT NULL,
    inn             VARCHAR(12)     NOT NULL UNIQUE,
    director_name   VARCHAR(255)    NOT NULL,
    phone           VARCHAR(20)     NOT NULL,
    email           VARCHAR(255)    NOT NULL,
    rating          INTEGER         NOT NULL DEFAULT 0
                        CONSTRAINT chk_rating CHECK (rating >= 0)
);

-- ============================================================
-- Таблица продукции ПЦБК
-- ============================================================
CREATE TABLE app.products (
    id              SERIAL          PRIMARY KEY,
    article         VARCHAR(50)     NOT NULL UNIQUE,
    product_name    VARCHAR(255)    NOT NULL,
    product_type    VARCHAR(100)    NOT NULL,
    min_price       NUMERIC(12,2)   NOT NULL
                        CONSTRAINT chk_price CHECK (min_price >= 0)
);

-- ============================================================
-- Таблица истории реализации
-- ============================================================
CREATE TABLE app.partner_sales (
    id              SERIAL          PRIMARY KEY,
    partner_id      INTEGER         NOT NULL
                        REFERENCES app.partners(id) ON DELETE CASCADE,
    product_id      INTEGER         NOT NULL
                        REFERENCES app.products(id) ON DELETE RESTRICT,
    quantity        INTEGER         NOT NULL
                        CONSTRAINT chk_qty CHECK (quantity > 0),
    sale_date       DATE            NOT NULL DEFAULT CURRENT_DATE
);

-- ============================================================
-- Индексы
-- ============================================================
CREATE INDEX idx_partners_type      ON app.partners(type_id);
CREATE INDEX idx_sales_partner      ON app.partner_sales(partner_id);
CREATE INDEX idx_sales_product      ON app.partner_sales(product_id);

-- ============================================================
-- Права для роли app
-- ============================================================
GRANT USAGE  ON SCHEMA app TO app;
GRANT ALL ON ALL TABLES    IN SCHEMA app TO app;
GRANT ALL ON ALL SEQUENCES IN SCHEMA app TO app;

-- ============================================================
-- Seed: Типы партнёров
-- ============================================================
INSERT INTO app.partner_types (type_name) VALUES
    ('ЗАО'), ('ООО'), ('ПАО'), ('ОАО'), ('ИП')
ON CONFLICT (type_name) DO NOTHING;

-- ============================================================
-- Seed: Продукция ПЦБК
-- ============================================================
INSERT INTO app.products (article, product_name, product_type, min_price) VALUES
    ('PCBK-001', 'Гофрокартон трёхслойный Т-23',    'Гофрокартон',  500.00),
    ('PCBK-002', 'Гофрокартон пятислойный Т-32',    'Гофрокартон',  700.00),
    ('PCBK-003', 'Бурая крафт-бумага 80 г/м²',      'Крафт-бумага', 600.00),
    ('PCBK-004', 'Тест-лайнер 125 г/м²',             'Бумага',       700.00),
    ('PCBK-005', 'Картон для плоских слоёв КЛ',     'Картон',       550.00),
    ('PCBK-006', 'Гофрокартон трёхслойный Т-24',    'Гофрокартон',  520.00),
    ('PCBK-007', 'Белая крафт-бумага 90 г/м²',      'Крафт-бумага', 650.00)
ON CONFLICT (article) DO NOTHING;

-- ============================================================
-- Seed: Партнёры ПЦБК
-- ============================================================
INSERT INTO app.partners
    (type_id, company_name, legal_address, inn,
     director_name, phone, email, rating)
VALUES
    (2, 'УралУпаковка',
        'г. Екатеринбург, ул. Малышева, 101',
        '6670123456', 'Козлов Дмитрий Александрович',
        '+73432345678', 'info@ural-upack.ru', 9),
    (5, 'ПермТара',
        'г. Пермь, ул. Героев Хасана, 34',
        '5904567890', 'Волкова Наталья Сергеевна',
        '+73422223344', 'permtara@yandex.ru', 7),
    (1, 'КаргоПак',
        'г. Тюмень, ул. Республики, 55',
        '7202098765', 'Новиков Андрей Игоревич',
        '+73452112233', 'cargopak@gmail.com', 8),
    (3, 'СибБумТорг',
        'г. Новосибирск, пр. Маркса, 7',
        '5407234567', 'Петрова Ирина Владимировна',
        '+73832445566', 'sibbum@inbox.ru', 6),
    (2, 'ЗападУрал Дистрибуция',
        'г. Пермь, Комсомольский пр., 70',
        '5902198765', 'Смирнов Евгений Петрович',
        '+73422667788', 'zudist@mail.ru', 10)
ON CONFLICT (inn) DO NOTHING;

-- ============================================================
-- Seed: История реализации продукции ПЦБК
-- ============================================================
INSERT INTO app.partner_sales (partner_id, product_id, quantity, sale_date) VALUES
    -- УралУпаковка (id=1)
    (1, 1,  5000,  '2024-01-15'),
    (1, 2,  8000,  '2024-03-20'),
    (1, 3, 12000,  '2024-06-10'),
    -- ПермТара (id=2)
    (2, 1,  3000,  '2024-02-14'),
    (2, 5,  1500,  '2024-04-05'),
    -- КаргоПак (id=3)
    (3, 1, 50000,  '2023-11-01'),
    (3, 2, 30000,  '2024-01-20'),
    (3, 4, 80000,  '2024-07-18'),
    -- СибБумТорг (id=4)
    (4, 3,  2000,  '2024-05-22'),
    -- ЗападУрал Дистрибуция (id=5)
    (5, 1, 150000, '2024-03-30'),
    (5, 2, 200000, '2024-08-11'),
    (5, 4,  60000, '2024-09-05');

-- ============================================================
-- Проверочный запрос
-- ============================================================
SELECT
    p.id,
    pt.type_name                              AS "Тип",
    p.company_name                            AS "Партнёр",
    p.rating                                  AS "Рейтинг",
    COALESCE(SUM(ps.quantity), 0)             AS "Объём реализации",
    CASE
        WHEN COALESCE(SUM(ps.quantity),0) >= 300000 THEN 15
        WHEN COALESCE(SUM(ps.quantity),0) >= 50000  THEN 10
        WHEN COALESCE(SUM(ps.quantity),0) >= 10000  THEN 5
        ELSE 0
    END                                       AS "Скидка %"
FROM app.partners p
JOIN app.partner_types pt ON pt.id = p.type_id
LEFT JOIN app.partner_sales ps ON ps.partner_id = p.id
GROUP BY p.id, pt.type_name
ORDER BY "Объём реализации" DESC;
