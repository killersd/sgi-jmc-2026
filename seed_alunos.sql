-- Execute conectado no banco "sgi-jmc" (o mesmo do appsettings.json da API)
-- psql -U postgres -d sgi-jmc -f seed_alunos.sql

INSERT INTO "Alunos"
    ("Nome", "Pai", "Mae", "DataNascimento", "Endereco", "Telefone",
     "NumeroDeclaracao", "CodigoSeed", "AnoLetivo", "AnoSerie", "Turma", "NumeroDoNis",
     "CorrecaoDeFluxo", "Transferido", "DataDeEmissao")
VALUES
    ('Maria Eduarda Souza',   'José Souza',      'Ana Souza',        '2014-03-12', 'Rua das Flores, 120',       '(85) 99111-2233', 0, 'MAT-2026-0001', 2026, 6, 'A', '16912345601', 'Não', false, now()),
    ('João Pedro Lima',       'Carlos Lima',     'Fernanda Lima',    '2013-07-25', 'Av. Central, 45',           '(85) 99222-3344', 0, 'MAT-2026-0002', 2026, 7, 'B', '16912345602', 'Não', false, now()),
    ('Ana Clara Ferreira',    'Roberto Ferreira','Patrícia Ferreira','2015-01-08', 'Rua do Sol, 88',            '(85) 99333-4455', 0, 'MAT-2026-0003', 2026, 5, 'A', '16912345603', 'Sim', false, now()),
    ('Lucas Gabriel Alves',   NULL,              'Simone Alves',     '2012-11-30', 'Travessa Nova, 15',         '(85) 99444-5566', 0, 'MAT-2026-0004', 2026, 8, 'A', '16912345604', 'Não', false, now()),
    ('Beatriz Santos Costa',  'Marcos Costa',    'Juliana Costa',    '2014-05-19', 'Rua das Palmeiras, 200',    '(85) 99555-6677', 0, 'MAT-2026-0005', 2026, 6, 'B', '16912345605', 'Não', false, now()),
    ('Pedro Henrique Rocha',  'André Rocha',     'Camila Rocha',     '2011-09-02', 'Av. Beira Mar, 500',        '(85) 99666-7788', 0, 'MAT-2026-0006', 2026, 9, 'A', '16912345606', 'Não', true,  now()),
    ('Sophia Martins Dias',   'Eduardo Dias',    'Renata Dias',      '2016-02-14', 'Rua João de Mattos, 33',    '(85) 99777-8899', 0, 'MAT-2026-0007', 2026, 4, 'U', '16912345607', 'Sim', false, now()),
    ('Gabriel Oliveira Rocha',NULL,              'Vanessa Oliveira', '2013-12-01', 'Rua das Acácias, 77',       '(85) 99888-9900', 0, 'MAT-2026-0008', 2026, 7, 'A', '16912345608', 'Não', false, now()),
    ('Isabela Cardoso Nunes', 'Fábio Cardoso',   'Larissa Cardoso',  '2014-08-22', 'Rua da Paz, 10',            '(85) 99999-0011', 0, 'MAT-2026-0009', 2026, 6, 'A', '16912345609', 'Não', false, now()),
    ('Matheus Barbosa Silva', 'Ricardo Barbosa', 'Adriana Barbosa',  '2012-04-17', 'Av. das Nações, 300',       '(85) 98111-2200', 0, 'MAT-2026-0010', 2026, 8, 'B', '16912345610', 'Não', false, now());
