aqui são os requisitos que eu pensei pra arquitetura de tudo

o trabalho será entregue em como um mono-repo, separando o front-end em react-native(Mudança de ultima hora, ignorar se dizer ao contrario), Back end: API REST em asp.NET core, a ORM pode ser EF Core, Banco de dados será um PostgreSQL,
quero um docker compose na pasta raíz que orquestre o front, o back e o banco de dados, e no diretorio de cada aplicação um outro compose que rode a aplicação sozinha, no do front ele roda apenas o front, no back roda o back e o banco, quero um dockerfile em cada respectiva aplicação pro compose principal orquestrar com base nos dockerfiles.
quero uma arquitetura hexagonal, seguindo os principios da clean architecture, eles podem ser flexiveis, desde que haja uma boa justificativa para violação de algum dos principios da mesma ou até do SOLID, que é crucial.
quero documentação com swagger.