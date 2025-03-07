toc.dat                                                                                             0000600 0004000 0002000 00000011643 14612204304 0014440 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        PGDMP        9                |            librarymanagement    16.1    16.1     �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false         �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false         �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false         �           1262    49877    librarymanagement    DATABASE     �   CREATE DATABASE librarymanagement WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_India.1252';
 !   DROP DATABASE librarymanagement;
                postgres    false         �            1259    49915    book    TABLE     P  CREATE TABLE public.book (
    id integer NOT NULL,
    bookname character varying(256) NOT NULL,
    author character varying(256),
    borrowerid integer,
    borrowername character varying(256),
    dateofissue timestamp without time zone,
    city character varying(256),
    genere character varying(256),
    isdeleted boolean
);
    DROP TABLE public.book;
       public         heap    postgres    false         �            1259    49914    book_id_seq    SEQUENCE     �   CREATE SEQUENCE public.book_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 "   DROP SEQUENCE public.book_id_seq;
       public          postgres    false    218         �           0    0    book_id_seq    SEQUENCE OWNED BY     ;   ALTER SEQUENCE public.book_id_seq OWNED BY public.book.id;
          public          postgres    false    217         �            1259    49908    borrower    TABLE     [   CREATE TABLE public.borrower (
    id integer NOT NULL,
    city character varying(256)
);
    DROP TABLE public.borrower;
       public         heap    postgres    false         �            1259    49907    borrower_id_seq    SEQUENCE     �   CREATE SEQUENCE public.borrower_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 &   DROP SEQUENCE public.borrower_id_seq;
       public          postgres    false    216         �           0    0    borrower_id_seq    SEQUENCE OWNED BY     C   ALTER SEQUENCE public.borrower_id_seq OWNED BY public.borrower.id;
          public          postgres    false    215         W           2604    49918    book id    DEFAULT     b   ALTER TABLE ONLY public.book ALTER COLUMN id SET DEFAULT nextval('public.book_id_seq'::regclass);
 6   ALTER TABLE public.book ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    218    217    218         V           2604    49911    borrower id    DEFAULT     j   ALTER TABLE ONLY public.borrower ALTER COLUMN id SET DEFAULT nextval('public.borrower_id_seq'::regclass);
 :   ALTER TABLE public.borrower ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    215    216    216         �          0    49915    book 
   TABLE DATA           t   COPY public.book (id, bookname, author, borrowerid, borrowername, dateofissue, city, genere, isdeleted) FROM stdin;
    public          postgres    false    218       4847.dat �          0    49908    borrower 
   TABLE DATA           ,   COPY public.borrower (id, city) FROM stdin;
    public          postgres    false    216       4845.dat �           0    0    book_id_seq    SEQUENCE SET     9   SELECT pg_catalog.setval('public.book_id_seq', 5, true);
          public          postgres    false    217         �           0    0    borrower_id_seq    SEQUENCE SET     =   SELECT pg_catalog.setval('public.borrower_id_seq', 3, true);
          public          postgres    false    215         [           2606    49922    book book_pkey 
   CONSTRAINT     L   ALTER TABLE ONLY public.book
    ADD CONSTRAINT book_pkey PRIMARY KEY (id);
 8   ALTER TABLE ONLY public.book DROP CONSTRAINT book_pkey;
       public            postgres    false    218         Y           2606    49913    borrower borrower_pkey 
   CONSTRAINT     T   ALTER TABLE ONLY public.borrower
    ADD CONSTRAINT borrower_pkey PRIMARY KEY (id);
 @   ALTER TABLE ONLY public.borrower DROP CONSTRAINT borrower_pkey;
       public            postgres    false    216         \           2606    49923    book book_borrowerid_fkey    FK CONSTRAINT     ~   ALTER TABLE ONLY public.book
    ADD CONSTRAINT book_borrowerid_fkey FOREIGN KEY (borrowerid) REFERENCES public.borrower(id);
 C   ALTER TABLE ONLY public.book DROP CONSTRAINT book_borrowerid_fkey;
       public          postgres    false    218    216    4697                                                                                                     4847.dat                                                                                            0000600 0004000 0002000 00000000554 14612204304 0014260 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        1	Geeta	Krishna	1	Meet	2024-04-28 00:00:00	Ahmedabad	Science Fiction	\N
2	Harry Potter	Harry	1	Meet	2024-04-25 00:00:00	Bhavnagar	Mystery	t
5	Dharti No Chhedo Ghar	Zaverchand Meghani	1	Meet	2024-04-25 00:00:00	Mumbai	Fantasy	\N
3	Culture	Dipendra Goyal	2	Margin	2024-04-27 00:00:00	Surat	Fantasy	\N
4	Ikigai	Unknow	3	Hasti	2024-04-30 00:00:00	Kuchh	Romance	t
\.


                                                                                                                                                    4845.dat                                                                                            0000600 0004000 0002000 00000000036 14612204304 0014251 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        2	Surat
3	Kuchh
1	Mumbai
\.


                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  restore.sql                                                                                         0000600 0004000 0002000 00000011011 14612204304 0015352 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        --
-- NOTE:
--
-- File paths need to be edited. Search for $$PATH$$ and
-- replace it with the path to the directory containing
-- the extracted data files.
--
--
-- PostgreSQL database dump
--

-- Dumped from database version 16.1
-- Dumped by pg_dump version 16.1

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

DROP DATABASE librarymanagement;
--
-- Name: librarymanagement; Type: DATABASE; Schema: -; Owner: postgres
--

CREATE DATABASE librarymanagement WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_India.1252';


ALTER DATABASE librarymanagement OWNER TO postgres;

\connect librarymanagement

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

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: book; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.book (
    id integer NOT NULL,
    bookname character varying(256) NOT NULL,
    author character varying(256),
    borrowerid integer,
    borrowername character varying(256),
    dateofissue timestamp without time zone,
    city character varying(256),
    genere character varying(256),
    isdeleted boolean
);


ALTER TABLE public.book OWNER TO postgres;

--
-- Name: book_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.book_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.book_id_seq OWNER TO postgres;

--
-- Name: book_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.book_id_seq OWNED BY public.book.id;


--
-- Name: borrower; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.borrower (
    id integer NOT NULL,
    city character varying(256)
);


ALTER TABLE public.borrower OWNER TO postgres;

--
-- Name: borrower_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.borrower_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.borrower_id_seq OWNER TO postgres;

--
-- Name: borrower_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.borrower_id_seq OWNED BY public.borrower.id;


--
-- Name: book id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.book ALTER COLUMN id SET DEFAULT nextval('public.book_id_seq'::regclass);


--
-- Name: borrower id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.borrower ALTER COLUMN id SET DEFAULT nextval('public.borrower_id_seq'::regclass);


--
-- Data for Name: book; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.book (id, bookname, author, borrowerid, borrowername, dateofissue, city, genere, isdeleted) FROM stdin;
\.
COPY public.book (id, bookname, author, borrowerid, borrowername, dateofissue, city, genere, isdeleted) FROM '$$PATH$$/4847.dat';

--
-- Data for Name: borrower; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.borrower (id, city) FROM stdin;
\.
COPY public.borrower (id, city) FROM '$$PATH$$/4845.dat';

--
-- Name: book_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.book_id_seq', 5, true);


--
-- Name: borrower_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.borrower_id_seq', 3, true);


--
-- Name: book book_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.book
    ADD CONSTRAINT book_pkey PRIMARY KEY (id);


--
-- Name: borrower borrower_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.borrower
    ADD CONSTRAINT borrower_pkey PRIMARY KEY (id);


--
-- Name: book book_borrowerid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.book
    ADD CONSTRAINT book_borrowerid_fkey FOREIGN KEY (borrowerid) REFERENCES public.borrower(id);


--
-- PostgreSQL database dump complete
--

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       