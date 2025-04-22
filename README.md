# cachedHash

Uma biblioteca .NET Standard para otimizar consultas através do uso de cache baseado nos parâmetros de entrada.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard-2.0-brightgreen.svg)](https://docs.microsoft.com/pt-br/dotnet/standard/net-standard)

## Visão Geral

`cachedHash` oferece uma maneira eficiente de armazenar em cache os resultados de operações (como consultas a bancos de dados) com base nos parâmetros de entrada. A biblioteca utiliza um hash dos parâmetros para gerar uma chave de cache única. Quando a mesma operação é solicitada com os mesmos parâmetros, o resultado é retornado do cache, evitando a necessidade de executar a operação novamente.

Esta biblioteca é agnóstica à implementação do cache subjacente. Você precisa fornecer uma implementação da interface `ICacheService<T>` para interagir com o seu sistema de cache preferido (memória, Redis, etc.).

## Como Usar

### Instalação

Como esta é uma biblioteca, você precisará referenciá-la em seu projeto .NET. Se você estiver usando o NuGet Package Manager ou a CLI do .NET, procure por `cachedHash` (após publicar o pacote) e instale-o.

```bash
dotnet add package cachedHash
