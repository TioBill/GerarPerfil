using Classes;
using ZwSoft.ZwCAD.DatabaseServices;

namespace Utils
{
    internal interface InfoGenInterface
    {
        Polyline GeraPontos(Polyline polyline, double distance, int index);
        DBText GeraNivelTerreno(Profile profile, double posicaoYTexto);
        DBText GerarProf(Profile profile, Position coords, string textString, double posicaoYTexto);
        DBText GeraDeclividade(Position position, double posicaoYTexto);
        DBText GeraExtensao(Position position, double posicaoYTexto);
        DBText GeraDistanciaProgressiva(Position position, double posicaoYTexto);
        DBText GeraNumeracaoEstacas(Position position, int index, Profile profile, double posicaoYTexto);
        DBText GeraGINivel(Profile profile, double posicaoYTexto, int index);
    }
}
