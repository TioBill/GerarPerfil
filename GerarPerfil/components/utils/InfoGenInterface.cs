using Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.Geometry;

using Classes;
using static Classes.Profile;
using System.Security.Cryptography;

namespace Utils
{
    internal interface InfoGenInterface
    {
        Polyline GeraPontos(Profile profile, double distance, int index);
        void GeraNivelTerrenoEProf(Profile profile, double posicaoYTexto, int scale = 10);
        void GeraDeclividade(Position position, double posicaoYTexto);
        void GeraExtensao(Position position, double posicaoYTexto);
        void GeraDistanciaProgressiva(Position position, double posicaoYTexto);
        void GeraNumeracaoEstacas(Position position, int index, Profile profile, double posicaoYTexto);
        void DesenhaLinhaEstacas(Profile profile);
        void GeraGINivel(Profile profile, double posicaoYTexto, int scale=10);
    }
}
