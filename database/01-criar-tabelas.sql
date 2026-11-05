USE db2025108027;

-- =========================================
-- 1. Tabela de condições dos jogos
-- =========================================
CREATE TABLE tb_condicao_jogo (
    co_condicao_jogo INT NOT NULL,
    ds_condicao_jogo VARCHAR(100) NOT NULL,

    CONSTRAINT pk_condicao_jogo
        PRIMARY KEY (co_condicao_jogo),

    CONSTRAINT uk_condicao_jogo_descricao
        UNIQUE (ds_condicao_jogo)
);

-- =========================================
-- 2. Tabela de perfis
-- =========================================
CREATE TABLE tb_perfil (
    co_perfil INT NOT NULL,
    ds_perfil VARCHAR(50) NOT NULL,

    CONSTRAINT pk_perfil
        PRIMARY KEY (co_perfil),

    CONSTRAINT uk_perfil_descricao
        UNIQUE (ds_perfil)
);

-- =========================================
-- 3. Tabela de plataformas
-- =========================================
CREATE TABLE tb_plataforma (
    co_seq_plataforma INT NOT NULL AUTO_INCREMENT,
    no_plataforma VARCHAR(100) NOT NULL,
    no_fabricante VARCHAR(100) NOT NULL,
    nu_ano_lancamento SMALLINT NOT NULL,

    CONSTRAINT pk_plataforma
        PRIMARY KEY (co_seq_plataforma),

    CONSTRAINT uk_plataforma_nome
        UNIQUE (no_plataforma)
);

-- =========================================
-- 4. Tabela de gêneros
-- =========================================
CREATE TABLE tb_genero (
    co_seq_genero INT NOT NULL AUTO_INCREMENT,
    no_genero VARCHAR(80) NOT NULL,

    CONSTRAINT pk_genero
        PRIMARY KEY (co_seq_genero),

    CONSTRAINT uk_genero_nome
        UNIQUE (no_genero)
);

-- =========================================
-- 5. Tabela de jogos
-- =========================================
CREATE TABLE tb_jogo (
    co_seq_jogo INT NOT NULL AUTO_INCREMENT,
    no_jogo VARCHAR(150) NOT NULL,
    ds_jogo TEXT NULL,
    nu_ano_lancamento SMALLINT NOT NULL,
    vl_preco DECIMAL(10,2) NOT NULL,
    qt_estoque INT NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    dt_cadastro DATETIME NOT NULL,
    co_plataforma INT NOT NULL,
    co_condicao_jogo INT NOT NULL,

    CONSTRAINT pk_jogo
        PRIMARY KEY (co_seq_jogo),

    INDEX idx_jogo_nome (no_jogo),

    CONSTRAINT fk_jogo_plataforma
        FOREIGN KEY (co_plataforma)
        REFERENCES tb_plataforma (co_seq_plataforma),

    CONSTRAINT fk_jogo_condicao
        FOREIGN KEY (co_condicao_jogo)
        REFERENCES tb_condicao_jogo (co_condicao_jogo)
);

-- =========================================
-- 6. Tabela associativa entre jogos e gêneros
-- =========================================
CREATE TABLE tb_jogo_genero (
    co_jogo INT NOT NULL,
    co_genero INT NOT NULL,

    CONSTRAINT pk_jogo_genero
        PRIMARY KEY (co_jogo, co_genero),

    CONSTRAINT fk_jogo_genero_jogo
        FOREIGN KEY (co_jogo)
        REFERENCES tb_jogo (co_seq_jogo),

    CONSTRAINT fk_jogo_genero_genero
        FOREIGN KEY (co_genero)
        REFERENCES tb_genero (co_seq_genero)
);

-- =========================================
-- 7. Tabela de usuários
-- =========================================
CREATE TABLE tb_usuario (
    co_seq_usuario INT NOT NULL AUTO_INCREMENT,
    no_usuario VARCHAR(120) NOT NULL,
    ds_email VARCHAR(254) NOT NULL,
    ds_senha_hash VARCHAR(255) NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    dt_cadastro DATETIME NOT NULL,
    co_perfil INT NOT NULL,

    CONSTRAINT pk_usuario
        PRIMARY KEY (co_seq_usuario),

    CONSTRAINT uk_usuario_email
        UNIQUE (ds_email),

    CONSTRAINT fk_usuario_perfil
        FOREIGN KEY (co_perfil)
        REFERENCES tb_perfil (co_perfil)
);