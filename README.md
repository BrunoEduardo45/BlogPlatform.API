# BlogPlatform.API

API RESTful para plataforma de blog desenvolvida com ASP.NET Core, em um padrão de arquitetura simplificado para facilitar o estudo e a minha evolução prática dos conceitos aprendidos em .NET, aplicando boas práticas de arquitetura, segurança e organização de código.  
A API está sendo construída de forma incremental, com o objetivo de construir uma API sólida, escalável e preparada para evoluir para um produto real.

---

# Tecnologias Utilizadas

## Até então

- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT (Json Web Token)
- Data Annotations
- Middleware de Autenticação e Autorização

## Próximas

- Swagger / OpenAPI
- Cache em memória
- Compressão de resposta
- Upload de arquivos

---

# Objetivo do Projeto

Construir uma API aplicando:

- Boas práticas de arquitetura
- Separação de responsabilidades
- Padronização de respostas para frontend
- Modelagem adequada com Entity Framework Core
- Autenticação e autorização seguras
- Organização de configurações por ambiente
- Preocupação com performance

---

# Arquitetura Atual

Estrutura organizada por responsabilidades:

- **Controllers**
- **Models** (Entidades)
- **ViewModels** (entrada e saída)
- **Data** (DbContext + Mappings (Fluent API))
- **Extensions** (padronização de respostas, tratamento de erros, extração e leitura das "Claims")
- **Services** (Autenticação / Token)
- **Configuration** (Configurações de ambiente, JWT, etc)

---

# Funcionalidades Implementadas

## Category

- CRUD completo
- Validação com Data Annotations em ViewModels
- Retorno padronizado

## Users

- Entidade User
- Armazenamento seguro de senha com Hash

## Autenticação

- Login via email e senha
- Geração de JWT
- Claims customizadas
- Controle de acesso por Roles
- Proteção de endpoints com `[Authorize]`

## Autenticação Alternativa via API Key

- Implementação de atributo personalizado
- Estratégia alternativa de autenticação
- Proteção adicional para endpoints específicos

---

# Próximos passos

## Configurações e Ambiente

- Organização do `appsettings.json`
- Separação por ambiente (Development / Production)
- Uso de `IConfiguration`
- Forçando HTTPS
- Configuração de Connection Strings

## Performance

- Paginação de dados
- Filtros em queries
- Cache em memória
- Compressão de resposta
- Ajuste de serialização padrão do ASP.NET

## Arquivos Estáticos e Upload

- Suporte a arquivos estáticos
- Upload de imagens
- Serviço preparado para envio de e-mails

## Documentação

- Swagger / OpenAPI habilitado
- Documentação interativa da API

## Outras funcionalidades que possam surgir durante o desenvolvimento.

---

# Padronização de Respostas

Todas as respostas seguem o padrão:

### Sucesso

```json
{
  "data": [
      {
        "id": 1,
        "name": "Category Name"
      },
      {
        "id": 2,
        "name": "Another Category"
      }
  ],
  "errors": []
}
```

### Erro

```json
{
  "data": null,
  "errors": [
    "Mensagem de erro"
  ]
}
```

---

# Autor

Desenvolvido por **Bruno Eduardo**

Projeto em sensevolvimento.