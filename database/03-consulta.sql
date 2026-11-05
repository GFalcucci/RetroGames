USE db2025108027;

-- =========================================================
-- ETAPA 5 - CONSULTAS
-- =========================================================


-- 1. CATÁLOGO
-- Jogos ativos com nome da plataforma e descrição da condição,
-- ordenados pelo nome do jogo.

SELECT
    j.no_jogo,
    p.no_plataforma,
    c.ds_condicao_jogo
FROM tb_jogo AS j
INNER JOIN tb_plataforma AS p
    ON j.co_plataforma = p.co_seq_plataforma
INNER JOIN tb_condicao_jogo AS c
    ON j.co_condicao_jogo = c.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo ASC;


-- 2. GÊNEROS DE UM JOGO INFORMADO
-- Altere o valor de @id_jogo para consultar outro jogo.

SET @id_jogo = 1;

SELECT
    j.no_jogo,
    g.no_genero
FROM tb_jogo_genero AS jg
INNER JOIN tb_jogo AS j
    ON jg.co_jogo = j.co_seq_jogo
INNER JOIN tb_genero AS g
    ON jg.co_genero = g.co_seq_genero
WHERE j.co_seq_jogo = @id_jogo
ORDER BY g.no_genero ASC;


-- 3. JOGOS DE UM GÊNERO INFORMADO
-- Altere o valor de @id_genero para consultar outro gênero.

SET @id_genero = 1;

SELECT
    g.no_genero,
    j.no_jogo
FROM tb_jogo_genero AS jg
INNER JOIN tb_genero AS g
    ON jg.co_genero = g.co_seq_genero
INNER JOIN tb_jogo AS j
    ON jg.co_jogo = j.co_seq_jogo
WHERE g.co_seq_genero = @id_genero
ORDER BY j.no_jogo ASC;


-- 4. QUANTIDADE DE JOGOS POR PLATAFORMA
-- Inclui plataformas que não possuem jogos, mostrando quantidade 0.

SELECT
    p.co_seq_plataforma,
    p.no_plataforma,
    COUNT(j.co_seq_jogo) AS quantidade_jogos
FROM tb_plataforma AS p
LEFT JOIN tb_jogo AS j
    ON j.co_plataforma = p.co_seq_plataforma
GROUP BY
    p.co_seq_plataforma,
    p.no_plataforma
ORDER BY p.no_plataforma ASC;


-- 5. BUSCA POR PARTE DO NOME DO JOGO
-- Retorna 5 itens por página.
-- Altere @parte_nome para pesquisar outro trecho.
-- Página 1: OFFSET 0
-- Página 2: OFFSET 5
-- Página 3: OFFSET 10

SET @parte_nome = 'Mario';

SELECT
    j.co_seq_jogo,
    j.no_jogo,
    j.vl_preco,
    j.qt_estoque,
    j.st_ativo
FROM tb_jogo AS j
WHERE j.no_jogo LIKE CONCAT('%', @parte_nome, '%')
ORDER BY j.no_jogo ASC
LIMIT 5 OFFSET 0;