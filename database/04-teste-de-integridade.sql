USE db2025108027;

-- =========================================================
-- ETAPA 5 - TESTES DE INTEGRIDADE
-- Execute cada teste separadamente.
-- Os erros esperados são intencionais.
-- =========================================================


-- TESTE 1: CHAVE ESTRANGEIRA
-- Tentar inserir um jogo com plataforma inexistente.
-- Resultado esperado: ERRO 1452.

INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
     qt_estoque, st_ativo, dt_cadastro,
     co_plataforma, co_condicao_jogo)
VALUES
    ('Jogo Teste FK', 'Teste de chave estrangeira.',
     2000, 50.00, 1, TRUE, NOW(), 9999, 1);


-- TESTE 2: VALOR ÚNICO
-- Tentar cadastrar uma plataforma com nome duplicado.
-- Resultado esperado: ERRO 1062.

INSERT INTO tb_plataforma
    (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
    ('Super Nintendo', 'Nintendo', 1990);


-- TESTE 3: EXCLUSÃO COM CHAVE ESTRANGEIRA
-- Tentar excluir uma plataforma que possui jogos.
-- Resultado esperado: ERRO 1451.

DELETE FROM tb_plataforma
WHERE co_seq_plataforma = 1;


-- TESTE 4: CAMPO OBRIGATÓRIO
-- Tentar inserir um jogo sem nome.
-- Resultado esperado: ERRO 1048.

INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
     qt_estoque, st_ativo, dt_cadastro,
     co_plataforma, co_condicao_jogo)
VALUES
    (NULL, 'Teste de campo obrigatório.',
     2000, 50.00, 1, TRUE, NOW(), 1, 1);


-- TESTE 5: PREÇO ZERO
-- Tentar inserir um jogo com preço igual a zero.
-- Resultado esperado: INSERÇÃO ACEITA.

INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
     qt_estoque, st_ativo, dt_cadastro,
     co_plataforma, co_condicao_jogo)
VALUES
    ('Jogo Teste Preco Zero',
     'Teste de preço igual a zero.',
     2000, 0.00, 1, TRUE, NOW(), 1, 1);


-- CONFERÊNCIA DO TESTE 5

SELECT *
FROM tb_jogo
WHERE no_jogo = 'Jogo Teste Preco Zero';


-- LIMPEZA DO REGISTRO DE TESTE
-- Executar depois de conferir a inserção.

DELETE FROM tb_jogo
WHERE no_jogo = 'Jogo Teste Preco Zero';