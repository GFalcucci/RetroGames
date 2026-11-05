USE db2025108027;

-- =========================================
-- 1. CONDIÇÕES DOS JOGOS
-- =========================================

INSERT INTO tb_condicao_jogo
    (co_condicao_jogo, ds_condicao_jogo)
VALUES
    (1, 'Novo'),
    (2, 'Usado'),
    (3, 'Recondicionado');


-- =========================================
-- 2. PERFIS
-- =========================================

INSERT INTO tb_perfil
    (co_perfil, ds_perfil)
VALUES
    (1, 'Administrador'),
    (2, 'Cliente');


-- =========================================
-- 3. PLATAFORMAS
-- =========================================

INSERT INTO tb_plataforma
    (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
    ('Super Nintendo', 'Nintendo', 1990),
    ('Mega Drive', 'Sega', 1988),
    ('PlayStation', 'Sony', 1994),
    ('Nintendo 64', 'Nintendo', 1996),
    ('Game Boy', 'Nintendo', 1989),
    ('Atari 2600', 'Atari', 1977);


-- =========================================
-- 4. GÊNEROS
-- =========================================

INSERT INTO tb_genero (no_genero)
VALUES
    ('Ação'),
    ('Aventura'),
    ('RPG'),
    ('Esporte'),
    ('Plataforma'),
    ('Luta'),
    ('Quebra-cabeça'),
    ('Corrida');


-- =========================================
-- 5. JOGOS
-- =========================================

INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
     qt_estoque, st_ativo, dt_cadastro,
     co_plataforma, co_condicao_jogo)
VALUES

    -- Super Nintendo (plataforma 1)

    ('Super Mario World', 'Clássico jogo de plataforma da Nintendo.', 1990, 149.90,
     5, TRUE, UTC_TIMESTAMP(), 1, 2),

    ('Donkey Kong Country', 'Aventura nas florestas com Donkey Kong.', 1994, 129.90,
     4, TRUE, UTC_TIMESTAMP(), 1, 2),

    ('Street Fighter II', 'Clássico jogo de luta.', 1991, 119.90,
     3, TRUE, UTC_TIMESTAMP(), 1, 2),

    ('Chrono Trigger', 'RPG clássico de viagem no tempo.', 1995, 199.90,
     2, TRUE, UTC_TIMESTAMP(), 1, 3),


    -- Mega Drive (plataforma 2)

    ('Sonic the Hedgehog', 'Primeira aventura clássica do Sonic.', 1991, 99.90,
     5, TRUE, UTC_TIMESTAMP(), 2, 2),

    ('Streets of Rage 2', 'Jogo de ação e combate nas ruas.', 1992, 109.90,
     3, TRUE, UTC_TIMESTAMP(), 2, 2),

    ('Mortal Kombat II', 'Clássico jogo de luta.', 1993, 119.90,
     2, TRUE, UTC_TIMESTAMP(), 2, 3),


    -- PlayStation (plataforma 3)

    ('Crash Bandicoot', 'Aventura do personagem Crash.', 1996, 149.90,
     4, TRUE, UTC_TIMESTAMP(), 3, 2),

    ('Metal Gear Solid', 'Jogo de ação e espionagem.', 1998, 169.90,
     2, TRUE, UTC_TIMESTAMP(), 3, 2),

    ('Resident Evil 2', 'Clássico jogo de sobrevivência e terror.', 1998, 159.90,
     3, TRUE, UTC_TIMESTAMP(), 3, 3),

    ('Gran Turismo', 'Simulador de corrida automobilística.', 1997, 139.90,
     2, TRUE, UTC_TIMESTAMP(), 3, 2),


    -- Nintendo 64 (plataforma 4)

    ('The Legend of Zelda Ocarina of Time', 'Aventura épica de Link.', 1998, 189.90,
     2, TRUE, UTC_TIMESTAMP(), 4, 2),

    ('GoldenEye 007', 'Jogo de ação e espionagem.', 1997, 139.90,
     3, TRUE, UTC_TIMESTAMP(), 4, 2),

    ('Mario Kart 64', 'Jogo de corrida com personagens da Nintendo.', 1996, 159.90,
     0, TRUE, UTC_TIMESTAMP(), 4, 2),


    -- Game Boy (plataforma 5)

    ('Pokemon Red', 'RPG clássico de captura de criaturas.', 1996, 179.90,
     4, TRUE, UTC_TIMESTAMP(), 5, 2),

    ('Tetris', 'Clássico jogo de quebra-cabeça.', 1989, 79.90,
     6, TRUE, UTC_TIMESTAMP(), 5, 1),

    ('The Legend of Zelda Links Awakening', 'Aventura portátil de Link.', 1993, 149.90,
     1, FALSE, UTC_TIMESTAMP(), 5, 3),


    -- Atari 2600 (plataforma 6)

    ('Pac-Man', 'Clássico jogo de labirinto.', 1982, 89.90,
     5, TRUE, UTC_TIMESTAMP(), 6, 2),

    ('Space Invaders', 'Jogo clássico de combate espacial.', 1978, 99.90,
     3, TRUE, UTC_TIMESTAMP(), 6, 2),

    ('Asteroids', 'Jogo clássico de destruição de asteroides.', 1979, 89.90,
     2, TRUE, UTC_TIMESTAMP(), 6, 2);


-- =========================================
-- 6. ASSOCIAÇÃO ENTRE JOGOS E GÊNEROS
-- =========================================

INSERT INTO tb_jogo_genero
    (co_jogo, co_genero)
VALUES

    -- Super Mario World
    (1, 5),
    (1, 2),

    -- Donkey Kong Country
    (2, 5),
    (2, 2),

    -- Street Fighter II
    (3, 6),
    (3, 1),

    -- Chrono Trigger
    (4, 3),
    (4, 2),

    -- Sonic the Hedgehog
    (5, 5),
    (5, 1),

    -- Streets of Rage 2
    (6, 1),
    (6, 6),

    -- Mortal Kombat II
    (7, 6),
    (7, 1),

    -- Crash Bandicoot
    (8, 5),
    (8, 2),

    -- Metal Gear Solid
    (9, 1),
    (9, 2),

    -- Resident Evil 2
    (10, 1),
    (10, 2),

    -- Gran Turismo
    (11, 8),
    (11, 4),

    -- Zelda Ocarina of Time
    (12, 2),
    (12, 3),

    -- GoldenEye 007
    (13, 1),
    (13, 2),

    -- Mario Kart 64
    (14, 8),
    (14, 4),

    -- Pokemon Red
    (15, 3),
    (15, 2),

    -- Tetris
    (16, 7),

    -- Zelda Links Awakening
    (17, 2),
    (17, 3),

    -- Pac-Man
    (18, 7),

    -- Space Invaders
    (19, 1),

    -- Asteroids
    (20, 1);