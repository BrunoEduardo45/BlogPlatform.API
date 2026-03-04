# BlogPlatform.API

API RESTful para plataforma de blog desenvolvida com ASP.NET Core, em um padrão de arquitetura simplificado para facilitar o estudo e a minha evolução prática dos conceitos aprendidos em .NET.

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

---

## Tecnologias Utilizadas

- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT (Json Web Token)
- Data Annotations
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
- **Data** (DbContext + Mappings (Fluent API))
- **Extensions** (padronização de respostas, tratamento de erros, extração e leitura das "Claims")
- **Services** (Autenticação / Token)
- **Configuration** (Configurações de ambiente, JWT, etc)

---

## Funcionalidades Implementadas

### Category

- CRUD completo
- Validação com Data Annotations em ViewModels
- Cache em memória
- Retorno padronizado

### Tags

- CRUD completo
- Associação e remoção de Tags em Posts
- Validação para impedir exclusão com relacionamento

### Posts

- CRUD completo
- Paginação
- Filtro por Categoria
- Filtro por Tag
- Controle de edição por autor ou admin

### Users / Accounts

- Registro de usuário
- Login com geração de JWT
- Logout
- Atualização de dados
- Remoção de conta (com validação de permissão)
- Listagem de usuários (admin)
- Consulta por Id
- Consulta por Role
- Upload e validação de imagem de perfil
- Armazenamento seguro de senha com Hash

### Roles

- CRUD completo
- Associação e remoção de Roles em usuários
- Controle de acesso baseado em Role (admin / author)
- Proteção contra remoção da role admin

### Autenticação

- Login via email e senha
- Geração de JWT
- Claims customizadas
- Controle de acesso por Roles
- Proteção de endpoints com `[Authorize]`

### Autenticação Alternativa via API Key

- Implementação de atributo personalizado
- Estratégia alternativa de autenticação
- Proteção adicional para endpoints específicos

### Performance

- Paginação de dados (Exemplo em Posts)
- Cache em memória (Exemplo em Categories)
- Compressão de resposta

### Arquivos Estáticos e Upload

- Suporte a arquivos estáticos
- Upload de imagens
- Serviço preparado para envio de e-mails

### Configurações e Ambiente

- Organização do `appsettings.json`
- Separação por ambiente (Development / Production)
- Forçando HTTPS
- Configuração de Connection Strings

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

## Autor

Desenvolvido por **Bruno Eduardo**

Codigo e commits criados em inglês para prática e evolução do idioma.