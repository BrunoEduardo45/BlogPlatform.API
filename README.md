# BlogPlatform.API

API RESTful para plataforma de blog desenvolvida com ASP.NET Core, em um padrão de arquitetura simplificado para facilitar o estudo e a minha evolução prática dos conceitos aprendidos em .NET.

Esta versão inclui melhorias estruturais, segurança aprimorada, seed inicial automatizado, estratégias de cache e refinamentos na documentação da API, aproximando o projeto de um cenário mais próximo de produção.

---

## Objetivo do Projeto

Construir uma API aplicando:

- Boas práticas de arquitetura
- Separação de responsabilidades
- Padronização de respostas para frontend
- Modelagem adequada com Entity Framework Core
- Autenticação e autorização seguras
- Organização de configurações por ambiente
- Preocupação com performance
- Implementação completa de controle de acesso baseado em Roles
- Aplicação de boas práticas de segurança e integridade de dados

---

## Tecnologias Utilizadas

- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT (Json Web Token)
- Data Annotations
- Fluent API
- Middleware de Autenticação e Autorização
- Upload de arquivos
- Cache em memória
- Compressão de resposta
- SecureIdentity.Password
- Swagger / OpenAPI

---

## Arquitetura Atual

Estrutura organizada por responsabilidades:

- **Controllers**
- **Models** (Entidades)
- **ViewModels** (entrada e saída)
- **Data** (DbContext + Mappings (Fluent API) + Seed inicial)
- **Extensions** (padronização de respostas, tratamento de erros, extração e leitura das "Claims")
- **Services** (Autenticação / Token)
- **Configuration** (Configurações de ambiente, JWT, etc)

---

## Funcionalidades Implementadas

### Category

- CRUD completo
- Validação com Data Annotations
- Cache em memória
- Retorno padronizado

### Tags

- CRUD completo
- Associação e remoção de Tags em Posts
- Validação para impedir exclusão com relacionamento

### Posts

- CRUD completo
- Paginação
- Filtro por Categoria, Tag, Title e Body com suporte a paginação
- Controle de edição por autor ou admin

### Users / Accounts

- CRUD completo
- Autenticação com JWT
- Upload e validação de imagem de perfil
- Hash seguro de senha
- Consulta por Id e Role
- Validações de permissão (remoção, edição, etc.)

### Roles

- CRUD completo
- Associação e remoção de Roles em usuários
- Controle de acesso baseado em Role (admin / author)
- Proteção contra remoção da role admin

### Segurança

- Login com geração de JWT
- Claims customizadas
- Proteção de endpoints com `[Authorize]`
- Autenticação alternativa via API Key (atributo personalizado)
- Endpoints internos ocultados do Swagger

### Performance

- Paginação de dados
- Cache em memória
- Compressão de resposta

### Arquivos e Infraestrutura

- Upload de imagens
- Suporte a arquivos estáticos
- Serviço preparado para envio de e-mails
- Configuração por ambiente (Development / Production)
- Forçando HTTPS
- Organização de Connection Strings
- Estrutura preparada para variáveis de ambiente

### Documentação

- Swagger / OpenAPI habilitado
- Documentação interativa da API

---

## Padronização de Respostas

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

## Execução do Projeto

Aplicar migrations e seed inicial:

``` Bash
dotnet ef database update
```

## Autor

Desenvolvido por **Bruno Eduardo**

Codigo e commits criados em inglês para prática e evolução do idioma.