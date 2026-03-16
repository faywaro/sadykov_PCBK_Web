--
-- PostgreSQL database dump
--

\restrict 9HCZdtKQctYrzA9EsQcmnqtx2ZdVRhbnB9udw8rl2xJX6RMvOOP9EGvUpSvNCJv

-- Dumped from database version 16.10
-- Dumped by pg_dump version 16.10

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: app; Type: SCHEMA; Schema: -; Owner: app
--

CREATE SCHEMA app;


ALTER SCHEMA app OWNER TO app;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: partner_sales; Type: TABLE; Schema: app; Owner: postgres
--

CREATE TABLE app.partner_sales (
    id integer NOT NULL,
    partner_id integer NOT NULL,
    product_id integer NOT NULL,
    quantity integer NOT NULL,
    sale_date date DEFAULT CURRENT_DATE NOT NULL,
    CONSTRAINT chk_qty CHECK ((quantity > 0))
);


ALTER TABLE app.partner_sales OWNER TO postgres;

--
-- Name: partner_sales_id_seq; Type: SEQUENCE; Schema: app; Owner: postgres
--

CREATE SEQUENCE app.partner_sales_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.partner_sales_id_seq OWNER TO postgres;

--
-- Name: partner_sales_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: postgres
--

ALTER SEQUENCE app.partner_sales_id_seq OWNED BY app.partner_sales.id;


--
-- Name: partner_types; Type: TABLE; Schema: app; Owner: postgres
--

CREATE TABLE app.partner_types (
    id integer NOT NULL,
    type_name character varying(100) NOT NULL
);


ALTER TABLE app.partner_types OWNER TO postgres;

--
-- Name: partner_types_id_seq; Type: SEQUENCE; Schema: app; Owner: postgres
--

CREATE SEQUENCE app.partner_types_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.partner_types_id_seq OWNER TO postgres;

--
-- Name: partner_types_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: postgres
--

ALTER SEQUENCE app.partner_types_id_seq OWNED BY app.partner_types.id;


--
-- Name: partners; Type: TABLE; Schema: app; Owner: postgres
--

CREATE TABLE app.partners (
    id integer NOT NULL,
    type_id integer NOT NULL,
    company_name character varying(255) NOT NULL,
    legal_address character varying(500) NOT NULL,
    inn character varying(12) NOT NULL,
    director_name character varying(255) NOT NULL,
    phone character varying(20) NOT NULL,
    email character varying(255) NOT NULL,
    rating integer DEFAULT 0 NOT NULL,
    CONSTRAINT chk_rating CHECK ((rating >= 0))
);


ALTER TABLE app.partners OWNER TO postgres;

--
-- Name: partners_id_seq; Type: SEQUENCE; Schema: app; Owner: postgres
--

CREATE SEQUENCE app.partners_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.partners_id_seq OWNER TO postgres;

--
-- Name: partners_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: postgres
--

ALTER SEQUENCE app.partners_id_seq OWNED BY app.partners.id;


--
-- Name: products; Type: TABLE; Schema: app; Owner: postgres
--

CREATE TABLE app.products (
    id integer NOT NULL,
    article character varying(50) NOT NULL,
    product_name character varying(255) NOT NULL,
    product_type character varying(100) NOT NULL,
    min_price numeric(12,2) NOT NULL,
    CONSTRAINT chk_price CHECK ((min_price >= (0)::numeric))
);


ALTER TABLE app.products OWNER TO postgres;

--
-- Name: products_id_seq; Type: SEQUENCE; Schema: app; Owner: postgres
--

CREATE SEQUENCE app.products_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.products_id_seq OWNER TO postgres;

--
-- Name: products_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: postgres
--

ALTER SEQUENCE app.products_id_seq OWNED BY app.products.id;


--
-- Name: partner_sales id; Type: DEFAULT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partner_sales ALTER COLUMN id SET DEFAULT nextval('app.partner_sales_id_seq'::regclass);


--
-- Name: partner_types id; Type: DEFAULT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partner_types ALTER COLUMN id SET DEFAULT nextval('app.partner_types_id_seq'::regclass);


--
-- Name: partners id; Type: DEFAULT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partners ALTER COLUMN id SET DEFAULT nextval('app.partners_id_seq'::regclass);


--
-- Name: products id; Type: DEFAULT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.products ALTER COLUMN id SET DEFAULT nextval('app.products_id_seq'::regclass);


--
-- Data for Name: partner_sales; Type: TABLE DATA; Schema: app; Owner: postgres
--

COPY app.partner_sales (id, partner_id, product_id, quantity, sale_date) FROM stdin;
1	1	1	5000	2024-01-15
2	1	2	8000	2024-03-20
3	1	3	12000	2024-06-10
4	2	1	3000	2024-02-14
5	2	5	1500	2024-04-05
6	3	1	50000	2023-11-01
7	3	2	30000	2024-01-20
8	3	4	80000	2024-07-18
9	4	3	2000	2024-05-22
10	5	1	150000	2024-03-30
11	5	2	200000	2024-08-11
12	5	4	60000	2024-09-05
\.


--
-- Data for Name: partner_types; Type: TABLE DATA; Schema: app; Owner: postgres
--

COPY app.partner_types (id, type_name) FROM stdin;
1	ЗАО
2	ООО
3	ПАО
4	ОАО
5	ИП
\.


--
-- Data for Name: partners; Type: TABLE DATA; Schema: app; Owner: postgres
--

COPY app.partners (id, type_id, company_name, legal_address, inn, director_name, phone, email, rating) FROM stdin;
1	2	УралУпаковка	г. Екатеринбург, ул. Малышева, 101	6670123456	Козлов Дмитрий Александрович	+73432345678	info@ural-upack.ru	9
2	5	ПермТара	г. Пермь, ул. Героев Хасана, 34	5904567890	Волкова Наталья Сергеевна	+73422223344	permtara@yandex.ru	7
3	1	КаргоПак	г. Тюмень, ул. Республики, 55	7202098765	Новиков Андрей Игоревич	+73452112233	cargopak@gmail.com	8
4	3	СибБумТорг	г. Новосибирск, пр. Маркса, 7	5407234567	Петрова Ирина Владимировна	+73832445566	sibbum@inbox.ru	6
5	2	ЗападУрал Дистрибуция	г. Пермь, Комсомольский пр., 70	5902198765	Смирнов Евгений Петрович	+73422667788	zudist@mail.ru	10
\.


--
-- Data for Name: products; Type: TABLE DATA; Schema: app; Owner: postgres
--

COPY app.products (id, article, product_name, product_type, min_price) FROM stdin;
1	PCBK-001	Гофрокартон трёхслойный Т-23	Гофрокартон	500.00
2	PCBK-002	Гофрокартон пятислойный Т-32	Гофрокартон	700.00
3	PCBK-003	Бурая крафт-бумага 80 г/м²	Крафт-бумага	600.00
4	PCBK-004	Тест-лайнер 125 г/м²	Бумага	700.00
5	PCBK-005	Картон для плоских слоёв КЛ	Картон	550.00
6	PCBK-006	Гофрокартон трёхслойный Т-24	Гофрокартон	520.00
7	PCBK-007	Белая крафт-бумага 90 г/м²	Крафт-бумага	650.00
8	321	321	321	321.00
\.


--
-- Name: partner_sales_id_seq; Type: SEQUENCE SET; Schema: app; Owner: postgres
--

SELECT pg_catalog.setval('app.partner_sales_id_seq', 13, true);


--
-- Name: partner_types_id_seq; Type: SEQUENCE SET; Schema: app; Owner: postgres
--

SELECT pg_catalog.setval('app.partner_types_id_seq', 5, true);


--
-- Name: partners_id_seq; Type: SEQUENCE SET; Schema: app; Owner: postgres
--

SELECT pg_catalog.setval('app.partners_id_seq', 6, true);


--
-- Name: products_id_seq; Type: SEQUENCE SET; Schema: app; Owner: postgres
--

SELECT pg_catalog.setval('app.products_id_seq', 8, true);


--
-- Name: partner_sales partner_sales_pkey; Type: CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partner_sales
    ADD CONSTRAINT partner_sales_pkey PRIMARY KEY (id);


--
-- Name: partner_types partner_types_pkey; Type: CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partner_types
    ADD CONSTRAINT partner_types_pkey PRIMARY KEY (id);


--
-- Name: partner_types partner_types_type_name_key; Type: CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partner_types
    ADD CONSTRAINT partner_types_type_name_key UNIQUE (type_name);


--
-- Name: partners partners_inn_key; Type: CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partners
    ADD CONSTRAINT partners_inn_key UNIQUE (inn);


--
-- Name: partners partners_pkey; Type: CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partners
    ADD CONSTRAINT partners_pkey PRIMARY KEY (id);


--
-- Name: products products_article_key; Type: CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.products
    ADD CONSTRAINT products_article_key UNIQUE (article);


--
-- Name: products products_pkey; Type: CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.products
    ADD CONSTRAINT products_pkey PRIMARY KEY (id);


--
-- Name: idx_partners_type; Type: INDEX; Schema: app; Owner: postgres
--

CREATE INDEX idx_partners_type ON app.partners USING btree (type_id);


--
-- Name: idx_sales_partner; Type: INDEX; Schema: app; Owner: postgres
--

CREATE INDEX idx_sales_partner ON app.partner_sales USING btree (partner_id);


--
-- Name: idx_sales_product; Type: INDEX; Schema: app; Owner: postgres
--

CREATE INDEX idx_sales_product ON app.partner_sales USING btree (product_id);


--
-- Name: partner_sales partner_sales_partner_id_fkey; Type: FK CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partner_sales
    ADD CONSTRAINT partner_sales_partner_id_fkey FOREIGN KEY (partner_id) REFERENCES app.partners(id) ON DELETE CASCADE;


--
-- Name: partner_sales partner_sales_product_id_fkey; Type: FK CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partner_sales
    ADD CONSTRAINT partner_sales_product_id_fkey FOREIGN KEY (product_id) REFERENCES app.products(id) ON DELETE RESTRICT;


--
-- Name: partners partners_type_id_fkey; Type: FK CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partners
    ADD CONSTRAINT partners_type_id_fkey FOREIGN KEY (type_id) REFERENCES app.partner_types(id) ON DELETE RESTRICT;


--
-- Name: TABLE partner_sales; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON TABLE app.partner_sales TO app;


--
-- Name: SEQUENCE partner_sales_id_seq; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON SEQUENCE app.partner_sales_id_seq TO app;


--
-- Name: TABLE partner_types; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON TABLE app.partner_types TO app;


--
-- Name: SEQUENCE partner_types_id_seq; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON SEQUENCE app.partner_types_id_seq TO app;


--
-- Name: TABLE partners; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON TABLE app.partners TO app;


--
-- Name: SEQUENCE partners_id_seq; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON SEQUENCE app.partners_id_seq TO app;


--
-- Name: TABLE products; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON TABLE app.products TO app;


--
-- Name: SEQUENCE products_id_seq; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON SEQUENCE app.products_id_seq TO app;


--
-- PostgreSQL database dump complete
--

\unrestrict 9HCZdtKQctYrzA9EsQcmnqtx2ZdVRhbnB9udw8rl2xJX6RMvOOP9EGvUpSvNCJv

