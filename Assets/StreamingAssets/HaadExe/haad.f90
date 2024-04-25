!!x86_64-w64-mingw32-gfortran.exe -ffree-form -ffree-line-length-0 -static -o haad.exe haad.f90 -O2
!!this program to add hydrogens into protein structure
!!it also can add side chain heavy atoms
!13 Jan 2009 intergrate the parameter into this code
program main
implicit none
integer,PARAMETER::NMAX=50000   !!max atom number
integer,PARAMETER::Lmax=3000   !!max residue number
integer,parameter::NSITT=10   !!maximum clash on the same sites
REAL,PARAMETER::pi=3.1415926,hpi=0.017453  !pi and pi/180.0
real,parameter::dismin=1e-6,dismin2=1e-3  !!the minimum cutoff 
real,DIMENSION(0:NMAX)::xx,yy,zz,xx0,yy0,zz0
real,DIMENSION(NMAX,4)::BOND    !record con1E,con1R ... con4E,con4R

character*1,dimension(NMAX)::ATY   !!record the atom type, H, N O,S, C
CHARACTER*4,DIMENSION(NMAX)::ATOMTY,atom !RECORD INPUT ATOMNAME
character*3,dimension(NMAX)::resd,resd2   !record the residue sequence name
character*4,dimension(20)::afn      !full name of 20 amino acids
character*30,dimension(NMAX)::fu    !for I/O out put

integer,dimension(20)::hra,ra      !record number of heavy atoms and atoms in each residue
!!3, 0 for no form, >0.5 to record on ip(onyl useful on H) of the acceptor
integer,DIMENSION(NMAX,5)::bonda    !atom connect,5 heavy atom bonded num

integer,dimension(NMAX)::ress,ress2       !record residue series number
integer,dimension(Lmax,2)::ca      !first record Ca atom index, 2nd record residue type in 20
integer,dimension(NMAX)::ASSrd  !!!record which atoms are added by this program
integer,dimension(Nmax)::tkfp,tkfpres  !!for tranfer

character*4,dimension(20,24)::atomname  
integer,dimension(20,24)::atomct !!connect type define in the array
character*4,dimension(33)::atomcontype
integer,dimension(Nmax)::atc  !1record the atomcontype for each atom
real,dimension(33,33)::bd0  !!bond length
real,dimension(33,33,33)::ag0 !angle
integer,dimension(24,4)::bdc

integer,dimension(50)::HH1,indx !!this record the atoms index for 1H within 5A, indx the sorted index
real,dimension(50)::rHH1 !!this for sorting index

    CHARACTER*500 SS,du,duu,f1,f2
    CHARACTER*54 kfp(Nmax)
    character*4 abc,prtype
    character bb

real shiftx,shifty,shiftz
real dx,dy,dz
real rk

real vr0(3),vr1(3,3)
real ax,ay,az,xi,yi,zi,x,y,z,r
real angg,fbd,bd,r1
real r12,r23,r13,bx,by,bz,cx,cy,cz
real r2,r3,phi,tab,tac,tbc,t1,ctbcx,ctbcy,ctbcz,ctbax,ctbay,ctbaz,t2
integer ipp(3),ipp0(3)
integer ires,iatom,ip1,in,iw,icc,ip2,ip3,ip4
integer ix,iy,iz,ix0,iy0,iz0,lxmin,lymin,lzmin,lxmax,lymax,lzmax,ip
integer ia1,ia2,ia3,ia4
integer ia,ib,ic,iflag,ir1,ir2
integer natom0  !!only heavy atoms
integer i,j,k,ii,kx,kk,kx0,j1
integer Nfbc,Ledge     !the side length of bondy simulation lattice and the edge,smallest box for one residue
integer NMAXx,NMAXy,NMAXz    !the largest box used in simulation, free boundary condition used:impetrable
integer nres,natom,nang,MHbond !number of atoms, angles, hbond bin width
integer ires0,ires1   !!!local residue assign and energy calculation use
integer iseed,ins,nnzr
integer i1,i2,i3,i4,i5,i6,i7,i8

integer iresin,Nhy
integer MH1  !the symbol +/- for the H redirection

      data afn/'GLY','ALA','SER','CYS','VAL','THR','ILE','PRO','MET','ASP','ASN','LEU','LYS','GLU','GLN','ARG','HIS','PHE','TYR','TRP'/
      data hra/4,5,6,6,7,7,8,7,8,8,8,8,9,9,9,11,10,11,12,14/   !!heavy atoms
      data ra/7,10,11,11,16,14,19,14,17,12,14,19,22,15,17,24,17,20,21,24/  !!whole atoms
      data (atomname(1,i),i=1,7)/" N  "," CA "," C  "," O  "," H  ","1HA ","2HA "/
      data (atomname(2,i),i=1,10)/" N  "," CA "," C  "," O  "," CB "," H  "," HA ","1HB ","2HB ","3HB "/ !ALA
      data (atomname(3,i),i=1,11)/" N  "," CA "," C  "," O  "," CB "," OG "," H  "," HA ","1HB ","2HB "," HG "/
      data (atomname(4,i),i=1,11)/" N  "," CA "," C  "," O  "," CB "," SG "," H  "," HA ","1HB ","2HB "," HG "/
      data (atomname(5,i),i=1,16)/" N  "," CA "," C  "," O  "," CB "," CG1"," CG2"," H  "," HA "," HB ","1HG1","2HG1","3HG1","1HG2","2HG2","3HG2"/
      data (atomname(6,i),i=1,14)/" N  "," CA "," C  "," O  "," CB "," OG1"," CG2"," H  "," HA "," HB "," HG1","1HG2","2HG2","3HG2"/
      data (atomname(7,i),i=1,19)/" N  "," CA "," C  "," O  "," CB "," CG1"," CG2"," CD1"," H  "," HA "," HB ","1HG1","2HG1","1HG2","2HG2","3HG2","1HD1","2HD1","3HD1"/
      data (atomname(8,i),i=1,14)/" N  "," CA "," C  "," O  "," CB "," CG "," CD "," HA ","1HB ","2HB ","1HG ","2HG ","1HD ","2HD "/
      data (atomname(9,i),i=1,17)/" N  "," CA "," C  "," O  "," CB "," CG "," SD "," CE "," H  "," HA ","1HB ","2HB ","1HG ","2HG ","1HE ","2HE ","3HE "/  !!MET
      data (atomname(10,i),i=1,12)/" N  "," CA "," C  "," O  "," CB "," CG "," OD1"," OD2"," H  "," HA ","1HB ","2HB "/  !!ASP
      data (atomname(11,i),i=1,14)/" N  "," CA "," C  "," O  "," CB "," CG "," OD1"," ND2"," H  "," HA ","1HB ","2HB ","1HD2","2HD2"/  !!ASN
      data (atomname(12,i),i=1,19)/" N  "," CA "," C  "," O  "," CB "," CG "," CD1"," CD2"," H  "," HA ","1HB ","2HB "," HG ","1HD1","2HD1","3HD1","1HD2","2HD2","3HD2"/  !!LEU
      data (atomname(13,i),i=1,22)/" N  "," CA "," C  "," O  "," CB "," CG "," CD "," CE "," NZ "," H  "," HA ","1HB ","2HB ","1HG ","2HG ","1HD ","2HD ","1HE ","2HE ","1HZ ","2HZ ","3HZ "/  !!LYS
      data (atomname(14,i),i=1,15)/" N  "," CA "," C  "," O  "," CB "," CG "," CD "," OE1"," OE2"," H  "," HA ","1HB ","2HB ","1HG ","2HG "/  !!GLU
      data (atomname(15,i),i=1,17)/" N  "," CA "," C  "," O  "," CB "," CG "," CD "," OE1"," NE2"," H  "," HA ","1HB ","2HB ","1HG ","2HG ","1HE2","2HE2"/  !!GLN
      data (atomname(16,i),i=1,24)/" N  "," CA "," C  "," O  "," CB "," CG "," CD "," NE "," CZ "," NH1"," NH2"," H  "," HA ","1HB ","2HB ","1HG ","2HG ","1HD ","2HD "," HE ","1HH1","2HH1","1HH2","2HH2"/  !!ARG
      data (atomname(17,i),i=1,17)/" N  "," CA "," C  "," O  "," CB "," CG "," ND1"," CD2"," CE1"," NE2"," H  "," HA ","1HB ","2HB "," HD1"," HD2"," HE1"/  !!HIS
      data (atomname(18,i),i=1,20)/" N  "," CA "," C  "," O  "," CB "," CG "," CD1"," CD2"," CE1"," CE2"," CZ "," H  "," HA ","1HB ","2HB "," HD1"," HD2"," HE1"," HE2"," HZ "/  !!PHE
      data (atomname(19,i),i=1,21)/" N  "," CA "," C  "," O  "," CB "," CG "," CD1"," CD2"," CE1"," CE2"," CZ "," OH "," H  "," HA ","1HB ","2HB "," HD1"," HD2"," HE1"," HE2"," HH "/  !!TYR
      data (atomname(20,i),i=1,24)/" N  "," CA "," C  "," O  "," CB "," CG "," CD1"," CD2"," NE1"," CE2"," CE3"," CZ2"," CZ3"," CH2"," H  "," HA ","1HB ","2HB "," HD1"," HE1"," HE3"," HZ2"," HZ3"," HH2"/  !!TRP
!!assign the atom connection type total 29 types    
     data (atomct(1,i),i=1,7)/10,2,4,26,17,19,19/
      data (atomct(2,i),i=1,10)/10,1,4,26,3,17,19,18,18,18/ !ALA
      data (atomct(3,i),i=1,11)/10,1,4,26,2,28,17,19,18,18,17/ !SER
      data (atomct(4,i),i=1,11)/10,1,4,26,2,29,17,19,18,18,21/  !CYS
      data (atomct(5,i),i=1,16)/10,1,4,26,1,3,3,17,19,18,18,18,18,18,18,18/ !VAL
      data (atomct(6,i),i=1,14)/10,1,4,26,1,28,3,17,19,18,17,18,18,18/ !THR
      data (atomct(7,i),i=1,19)/10,1,4,26,1,2,3,3,17,19,18,18,18,18,18,18,18,18,18/ !ILE
      data (atomct(8,i),i=1,14)/9,30,4,26,31,31,32,19,18,18,18,18,18,18/ !PRO
      data (atomct(9,i),i=1,17)/10,1,4,26,2,2,29,3,17,19,18,18,18,18,18,18,18/  !!MET
      data (atomct(10,i),i=1,12)/10,1,4,26,2,5,27,27,17,19,18,18/  !!ASP
      data (atomct(11,i),i=1,14)/10,1,4,26,2,5,26,11,17,19,18,18,17,17/  !!ASN
      data (atomct(12,i),i=1,19)/10,1,4,26,2,1,3,3,17,19,18,18,18,18,18,18,18,18,18/  !!LEU
      data (atomct(13,i),i=1,22)/10,1,4,26,2,2,2,2,12,17,19,18,18,18,18,18,18,18,18,20,20,20/  !!LYS
      data (atomct(14,i),i=1,15)/10,1,4,26,2,2,5,27,27,17,19,18,18,18,18/  !!GLU
      data (atomct(15,i),i=1,17)/10,1,4,26,2,2,5,26,11,17,19,18,18,18,18,17,17/  !!GLN
      data (atomct(16,i),i=1,24)/10,1,4,26,2,2,2,13,4,13,13,17,19,18,18,18,18,18,18,20,20,20,20,20/  !!ARG
      data (atomct(17,i),i=1,17)/10,1,4,26,2,6,15,6,7,15,17,19,18,18,17,22,22/  !!HIS
      data (atomct(18,i),i=1,20)/10,1,4,26,2,23,23,23,23,23,23,17,19,18,18,25,25,25,25,25/  !!PHE
      data (atomct(19,i),i=1,21)/10,1,4,26,2,23,23,23,23,23,23,28,17,19,18,18,25,25,25,25,17/  !!TYR
      data (atomct(20,i),i=1,24)/10,1,4,26,2,14,23,8,16,8,23,23,23,23,17,19,18,18,25,17,25,25,25,25/ !!TRP
!!                       1     2    3     4   5     6      7     8     9   10    11    12    13     14   15    16 17
      data atomcontype/"CT1","CT2","CT3","C","CC","CPH1","CPH2","CPT","N","NH1","NH2","NH3","NC2","CY","NR2","NY","H","HA","HB","HC","HS","HR1","CA","HR3","HP","O","OC","OH1","S","CP1","CP2","CP3","NP"/  ! atom types 8+8+9+4
!18   19   20   21   22    23    24   25   26  27   28   29   30    31    32    33(P_Nterm)
!!bond length assign in 29*29 matrix, angle anssign in 29*29*29 matrix 

 f1=''
call getarg(1,f1)

if(len(trim(f1))<2)then
print*,"USING Guide::"
print*,"./haad  xx.pdb (ADD H only, output: xx.pdb.h)"
goto 777
endif

!!read pdb structure *************************FULL ATOMIC***************************************
1101 format(A100)
1102 format(A30)
    open(1,file=trim(f1))   !!!only for test
        in=0
        ir1=1
30  read(1,1101,end=31)ss     !!F1 structure file
      if(ss(1:3).eq.'END'.or.ss(1:3).eq.'TER') goto 31
      bb=ss(17:17)
      if(ss(1:4).eq.'ATOM'.and.(bb.eq.' '.or.bb.eq.'A'))then
         in=in+1
         read(ss,101)du,atom(in),duu,resd(in),duu,ir2,duu,xx0(in),yy0(in),zz0(in)     
         read(ss,1102)kfp(in)
         tkfpres(in)=ir2
         if(ir2>ic.and.in>1)ir1=ir1+1
         ress(in)=ir1     
         resd2(ir1)=resd(in)  !!get the residue sequence
         ic=ir2
      endif
      goto 30
31   continue   
      close(1)
      if(in<5)then
         print*,"read pdb file err, total atoms",in
         stop
      endif
      natom0=in
      Nres=ir1
!!!according to ress build the force field
 
!!assign the known coordinates into xyz
call getresin  !assign the atomname, atom index and CA

xx=999.0
yy=999.0
zz=999.0
tkfp=0
do i=1,natom  !!should be charmm atm file
   do j=1,natom0
      if(atomty(i).eq.atom(j).and.ress2(i).eq.ress(j))then
         xx(i)=xx0(j)
         yy(i)=yy0(j)
         zz(i)=zz0(j)
         tkfp(i)=j
         goto 11
      endif
   enddo
11 continue
enddo


!!count the number of heavy atoms bonded to each atom
do i=1,natom
      kx0=0
   do k=1,4
      ii=bonda(i,k)
      if(ii>0)then
         if(aty(ii).eq.'H')cycle
         kx0=kx0+1
      endif
   enddo
   bonda(i,5)=kx0  !number of heavy atoms bonded
   ress(i)=ress2(i)
enddo
!do i=1,Natom
!   write(*,"A4,5I5,A4,A5"),"tstk",i,ress(i),ress2(i),ca(ress(i),1),ca(ress(i),2),resd(i),atomty(i)
!enddo


Assrd=0
iseed=natom 

101 format(A12,A4,A1,A3,A2,I4,A4,3F8.3)   !pdb
772 continue

   call heavy_ADD
   call HADD
   f2=trim(f1)//".h"
   open(10,file=f2)
   do i=1,natom
      k=tkfp(i)
 !     if(xx(i)>990.0)cycle
      if(k>1)ir2=tkfpres(k)
      if(ASSrd(i)>0)then
         write(10,129),0,atomty(i),resd(i),ir2,xx(i),yy(i),zz(i),ASSrd(i)         
      elseif(k>0)then
         write(10,1103),kfp(k),xx(i),yy(i),zz(i)
      endif
   enddo
   write(10,119)
   close(10)
1103 format(A30,3f8.3)
119 format("TER")
129 format("ATOM",I7,1X,A4,1X,A3,2X,I4,4X,3F8.3,I3)   !pdb
777 continue
contains   !!6 and 66 done

!!this subroutine to add those misssing heavy atoms
!!except CB, CG and CD
subroutine HEAVY_ADD
implicit none
!scan heavy atoms at first
do i=1,natom
   if(aty(i).eq.'H'.or.xx(i)<990)cycle
        ASSrd(i)=1
        ires=ress(i)
        do k=1,4
           ii=bonda(i,k)
           if(ii<1)goto 51
           if(xx(ii)>990)cycle 
           icc=ii
           bd=bond(i,k)
           goto 51
        enddo
51      continue
        ax=xx(icc)  !!origin point
        ay=yy(icc)
        az=zz(icc)
!write(*,"A10,I5,A4,A4,I4,3f8.3"),"readd heavy",i,atomty(i),resd(i),ress(i),ax,ay,az
        kx0=0
        ip2=0
        do j=1,4 !!the connect heavy atom  !!ipx-ip2-icc-H (1,2,3)
           ii=bonda(icc,j)
           if(ii<1)goto 301
           if(xx(ii)>990.or.ii.eq.i.or.aty(ii).eq."H")cycle
           ip2=ii
           kx0=kx0+1
           ipp0(kx0)=ip2
        enddo
301     continue
        i1=atc(i)
        i2=atc(icc)
        i3=atc(ip2)
        if(i1>i3)then
           i1=atc(ip2)
           i3=atc(i)
        endif
        angg=ag0(i1,i2,i3)*hpi

        kx=0
        do j=1,4
           ii=bonda(ip2,j)
           if(ii<1.or.(ii==icc.or.aty(ii)=='H'.or.xx(ii)>990))cycle
           kx=kx+1
           ipp(kx)=ii
        enddo
        ip3=ipp(1)
!treat heavy atoms missing !ip3-ip2-icc-i (heavy atom)
!!C-term ALA-Cb; ILE Cd; LEU Cd1,Cd2;MET Ce; Val CG1; THR CG2  sp3
!!O-term O for all, ASN OD1; ASP OD1, OD2; GLN OE1; GLU OE1,OE2; TYR OH;sp2 || SER OG; THR OG1, OXT (sp3) 
!!S-term CYS SG sp3
!!N-term N-term LYS NZ sp3, ARG NH1,NH2; ASN ND2; GLN NE2 sp2
        abc=atomty(i)  !for sp2 second atom      
  if(bonda(i,5).eq.1)then 
     if(abc.eq.' OD2'.or.abc.eq.' OE2'.or.abc.eq.' ND2'.or.abc.eq.' NE2'.or.abc.eq.' NH2'.or.abc.eq.' OH '.or.abc.eq.' OXT')then
!print*,"addheavy",abc,i
        if(abc.eq.' OH ')then
           ip2=icc-1
           ip3=icc-2
        else !if(abc.eq." OXT".or.abc.eq." NH2")then
           ip2=icc-1
           ip3=icc+1
        endif
        x=0.5*(xx(ip2)+xx(ip3))
        y=0.5*(yy(ip2)+yy(ip3))
        z=0.5*(zz(ip2)+zz(ip3))
        r1=sqrt((x-ax)*(x-ax)+(y-ay)*(y-ay)+(z-az)*(z-az))
        if(r1>dismin2)then
           fbd=bd/r1
           xx(i)=ax+fbd*(ax-x)
           yy(i)=ay+fbd*(ay-y)
           zz(i)=az+fbd*(az-z)
        else
           ASSrd(i)=2           
      !     print*,"warn!ADD Cterm1",i,atomty(i),ip3,ip2,icc
           ip3=ipp(1)
           xx(i)=ax+xx(ip2)-xx(ip3)
           yy(i)=ay+yy(ip2)-yy(ip3)
           zz(i)=az+zz(ip2)-zz(ip3)
        endif
     elseif(abc.eq.' CD2'.and.resd(i).eq.'LEU')then ! LEU
        xi=ax-xx(icc-1)
        yi=ay-yy(icc-1)
        zi=az-zz(icc-1)
        x=xx(i-1) !CD1
        y=yy(i-1)
        z=zz(i-1)
        call rotate_matrix2(x,y,z,ax,ay,az,xi,yi,zi,2.18166)  !125o
        xx(i)=x
        yy(i)=y
        zz(i)=z  
     elseif(abc.eq.' CG2')then !VAL, THR, ILE
        xi=ax-xx(icc-3)
        yi=ay-yy(icc-3)
        zi=az-zz(icc-3)
        x=xx(i-1) !CD1
        y=yy(i-1)
        z=zz(i-1)
        call rotate_matrix2(x,y,z,ax,ay,az,xi,yi,zi,2.18166)  !125o
        xx(i)=x
        yy(i)=y
        zz(i)=z  
!        print*,"CG2",atomty(i),xx(i),yy(i),zz(i) 
     elseif((abc.eq.' CD1'.and.resd(i).eq."ILE").or.(abc.eq.' CE '.and.resd(i).eq."MET"))then !ILE
        xi=xx(icc-1)-0.5*(xx(i-1)+xx(icc-4))
        yi=yy(icc-1)-0.5*(yy(i-1)+yy(icc-4))
        zi=zz(icc-1)-0.5*(zz(i-1)+zz(icc-4))
        x=xi-ax
        y=yi-ay
        z=zi-az
        r1=sqrt(x*x+y*y+z*z)
        if(r1<0.1)r1=0.1
        fbd=bd/r1
        xx(i)=ax+fbd*x
        yy(i)=ay+fbd*y
        zz(i)=az+fbd*z      
     else  !!sp3 and sp2 first atom  ?????????
        if(abc.eq.' CG1'.or.abc.eq.' SG '.or.abc.eq.' OG1'.or.abc.eq.' OG ')ip3=ip2+1
!     print*,"HADD>=",ress(i),atomty(ip3),atomty(ip2),atomty(i),resd(i),abc
        x=xx(ip2)-xx(ip3)
        y=yy(ip2)-yy(ip3)
        z=zz(ip2)-zz(ip3)
        r1=sqrt(x*x+y*y+z*z)
        if(r1>dismin2)then
           fbd=bd/r1
           xx(i)=ax+fbd*x
           yy(i)=ay+fbd*y
           zz(i)=az+fbd*z
        else
           ASSrd(i)=2
        !   print*,"warn!ADD Cterm2",i,atomty(i),ip3,ip2,icc
           xx(i)=2.0*ax-xx(ip2)
           yy(i)=2.0*ay-yy(ip2)
           zz(i)=2.0*az-zz(ip2)
        endif   
     endif
  elseif(bonda(i,5)>=2)then  !!treat on more atoms missing
     if(abc.eq.' CG ')ip3=ip2+1
     x=xx(ip2)-xx(ip3)
     y=yy(ip2)-yy(ip3)
     z=zz(ip2)-zz(ip3)
     r1=sqrt(x*x+y*y+z*z)
     if(r1>dismin2)then
        fbd=bd/r1
        xx(i)=ax+fbd*x
        yy(i)=ay+fbd*y
        zz(i)=az+fbd*z
     else
        ASSrd(i)=2
     !   print*,"warn!ADD Cterm3",i,atomty(i),ip3,ip2,icc
        xx(i)=ax+x
        yy(i)=ay+y
        zz(i)=az+z
     endif  !!default set
if(.not.(ca(ress(i),2).eq.8.or.ca(ress(i),2)>=17))cycle

!print*,atomty(i),resd(i),ress(i),"<<<<ring??"
if(abc.eq." CG ")then
   if(resd(i).eq."PRO")then   !!this region treat the ring PRO CG-CD
      x=xx(i)-xx(ip2-1)
      y=yy(i)-yy(ip2-1)
      z=zz(i)-zz(ip2-1)
      r1=sqrt(x*x+y*y+z*z)
      if(r1<dismin2)r1=dismin2
      fbd=2.22/r1
      xx(i)=xx(ip2-1)+fbd*x  !CG
      yy(i)=yy(ip2-1)+fbd*y
      zz(i)=zz(ip2-1)+fbd*z
      x=0.5*(xx(ip2-1)+xx(i)-xx(ip2)-xx(i-1))
      y=0.5*(yy(ip2-1)+yy(i)-yy(ip2)-yy(i-1))
      z=0.5*(zz(ip2-1)+zz(i)-zz(ip2)-zz(i-1))
      xx(i+1)=0.5*(xx(ip2)+xx(i-1))+1.618*x
      yy(i+1)=0.5*(yy(ip2)+yy(i-1))+1.618*y
      zz(i+1)=0.5*(zz(ip2)+zz(i-1))+1.618*z
      ASSrd(i+1)=1
   else
      bx=xx(ip2+1)-xx(ip2-1)
      by=yy(ip2+1)-yy(ip2-1)
      bz=zz(ip2+1)-zz(ip2-1)
      cx=xx(i)-xx(i-1)
      cy=yy(i)-yy(i-1)
      cz=zz(i)-zz(i-1)
      ctbcx=by*cz-bz*cy      !cross product  bxc
      ctbcy=bz*cx-bx*cz
      ctbcz=bx*cy-by*cx
      r1=sqrt(ctbcx*ctbcx+ctbcy*ctbcy+ctbcz*ctbcz)
      if(r1<dismin2)r1=dismin2
   if(resd(i).eq."PHE".or.resd(i).eq."TYR")then !.or.resd(i).eq."TYR"))then
      xx(i+5)=xx(i)+1.788*cx
      yy(i+5)=yy(i)+1.788*cy
      zz(i+5)=zz(i)+1.788*cz
      xi=xx(i+5)-xx(i)
      yi=yy(i+5)-yy(i)
      zi=zz(i+5)-zz(i)
      fbd=1.191/r1
      xx(i+1)=xx(i)+0.25*xi+ctbcx*fbd
      yy(i+1)=yy(i)+0.25*yi+ctbcy*fbd
      zz(i+1)=zz(i)+0.25*zi+ctbcz*fbd
      xx(i+2)=xx(i)+0.25*xi-ctbcx*fbd
      yy(i+2)=yy(i)+0.25*yi-ctbcy*fbd
      zz(i+2)=zz(i)+0.25*zi-ctbcz*fbd
      xx(i+3)=xx(i)+0.75*xi+ctbcx*fbd
      yy(i+3)=yy(i)+0.75*yi+ctbcy*fbd
      zz(i+3)=zz(i)+0.75*zi+ctbcz*fbd
      xx(i+4)=xx(i)+0.75*xi-ctbcx*fbd
      yy(i+4)=yy(i)+0.75*yi-ctbcy*fbd
      zz(i+4)=zz(i)+0.75*zi-ctbcz*fbd
      if(resd(i).eq."TYR")then
         xx(i+6)=xx(i+5)+0.937*cx  !!OH
         yy(i+6)=yy(i+5)+0.937*cy
         zz(i+6)=zz(i+5)+0.937*cz
      endif
      ASSrd(i+1)=1
      ASSrd(i+2)=1
      ASSrd(i+3)=1
      ASSrd(i+4)=1
   elseif(resd(i).eq."TRP")then
      fbd=1.19/r1
      xx(i+1)=xx(i)+0.63*cx+fbd*ctbcx !CD1
      yy(i+1)=yy(i)+0.63*cy+fbd*ctbcy
      zz(i+1)=zz(i)+0.63*cz+fbd*ctbcz
      xx(i+2)=xx(i)+0.448*cx-fbd*ctbcx !!CD2
      yy(i+2)=yy(i)+0.448*cy-fbd*ctbcy
      zz(i+2)=zz(i)+0.448*cz-fbd*ctbcz
      fbd=0.688/r1
      xx(i+3)=xx(i+1)+0.819*cx-fbd*ctbcx  !!NE1
      yy(i+3)=yy(i+1)+0.819*cy-fbd*ctbcy
      zz(i+3)=zz(i+1)+0.819*cz-fbd*ctbcz
      xx(i+4)=xx(i+2)+0.774*cx+fbd*ctbcx !!CE2
      yy(i+4)=yy(i+2)+0.774*cy+fbd*ctbcy
      zz(i+4)=zz(i+2)+0.774*cz+fbd*ctbcz
      fbd=1.19/r1
      xx(i+5)=xx(i+2)-fbd*ctbcx-0.448*cx !CE3
      yy(i+5)=yy(i+2)-fbd*ctbcy-0.448*cy
      zz(i+5)=zz(i+2)-fbd*ctbcz-0.448*cz 
      xx(i+6)=xx(i+4)-fbd*ctbcx+0.448*cx !CZ2
      yy(i+6)=yy(i+4)-fbd*ctbcy+0.448*cy
      zz(i+6)=zz(i+4)-fbd*ctbcz+0.448*cz      
      xx(i+7)=xx(i+5)-fbd*ctbcx+0.448*cx !CZ3
      yy(i+7)=yy(i+5)-fbd*ctbcy+0.448*cy
      zz(i+7)=zz(i+5)-fbd*ctbcz+0.448*cz      
      xx(i+8)=xx(i+7)+xx(i+4)-xx(i+2)    !CH2
      yy(i+8)=yy(i+7)+yy(i+4)-yy(i+2)
      zz(i+8)=zz(i+7)+zz(i+4)-zz(i+2)  
      do j=1,8
         ASSrd(i+j)=1
      enddo
   elseif(resd(i).eq."HIS")then
      fbd=1.19/r1
      xx(i+1)=xx(i)+0.6*cx+fbd*ctbcx  !!ND1
      yy(i+1)=yy(i)+0.6*cy+fbd*ctbcy
      zz(i+1)=zz(i)+0.6*cz+fbd*ctbcz
      xx(i+2)=xx(i)+0.63*cx-fbd*ctbcx !!CD2
      yy(i+2)=yy(i)+0.63*cy-fbd*ctbcy
      zz(i+2)=zz(i)+0.63*cz-fbd*ctbcz
      fbd=0.688/r1
      xx(i+3)=xx(i)+1.35*cx+fbd*ctbcx  !!CE1
      yy(i+3)=yy(i)+1.35*cy+fbd*ctbcy
      zz(i+3)=zz(i)+1.35*cz+fbd*ctbcz
      xx(i+4)=xx(i)+1.41*cx-fbd*ctbcx !!NE2
      yy(i+4)=yy(i)+1.41*cy-fbd*ctbcy
      zz(i+4)=zz(i)+1.41*cz-fbd*ctbcz
      do j=1,4
         ASSrd(i+j)=1
      enddo
   endif
endif
elseif(abc.eq." CD ".or.abc.eq." CD1")then  
   if(resd(i).eq."PRO")then   !!this region treat the ring PRO CG-CD
      ip3=ca(ress(i),1)  !!Ca
      x=0.5*(xx(ip3-1)+xx(i-1)-xx(ip3)-xx(i-2))
      y=0.5*(yy(ip3-1)+yy(i-1)-yy(ip3)-yy(i-2))
      z=0.5*(zz(ip3-1)+zz(i-1)-zz(ip3)-zz(i-2))
      xx(i+1)=0.5*(xx(ip3)+xx(i-2))+1.618*x
      yy(i+1)=0.5*(yy(ip3)+yy(i-2))+1.618*y
      zz(i+1)=0.5*(zz(ip3)+zz(i-2))+1.618*z
      ASSrd(i)=1
   else
      bx=xx(ip3+1)-xx(ip3-1)
      by=yy(ip3+1)-yy(ip3-1)
      bz=zz(ip3+1)-zz(ip3-1)
      cx=xx(i-1)-xx(i-2)
      cy=yy(i-1)-yy(i-2)
      cz=zz(i-1)-zz(i-2)
      ctbcx=by*cz-bz*cy      !cross product  bxc
      ctbcy=bz*cx-bx*cz
      ctbcz=bx*cy-by*cx
      r1=sqrt(ctbcx*ctbcx+ctbcy*ctbcy+ctbcz*ctbcz)
      if(r1<dismin2)r1=dismin2
      if(resd(i).eq."PHE".or.resd(i).eq."TYR")then !!
         xx(i+4)=xx(i-1)+1.788*cx
         yy(i+4)=yy(i-1)+1.788*cy
         zz(i+4)=zz(i-1)+1.788*cz
         xi=xx(i+4)-xx(i-1)
         yi=yy(i+4)-yy(i-1)
         zi=zz(i+4)-zz(i-1)
         fbd=1.191/r1
         xx(i)=xx(i-1)+0.25*xi+ctbcx*fbd
         yy(i)=yy(i-1)+0.25*yi+ctbcy*fbd
         zz(i)=zz(i-1)+0.25*zi+ctbcz*fbd
         xx(i+1)=xx(i-1)+0.25*xi-ctbcx*fbd
         yy(i+1)=yy(i-1)+0.25*yi-ctbcy*fbd
         zz(i+1)=zz(i-1)+0.25*zi-ctbcz*fbd
         xx(i+2)=xx(i-1)+0.75*xi+ctbcx*fbd
         yy(i+2)=yy(i-1)+0.75*yi+ctbcy*fbd
         zz(i+2)=zz(i-1)+0.75*zi+ctbcz*fbd
         xx(i+3)=xx(i-1)+0.75*xi-ctbcx*fbd
         yy(i+3)=yy(i-1)+0.75*yi-ctbcy*fbd
         zz(i+3)=zz(i-1)+0.75*zi-ctbcz*fbd
         if(resd(i).eq."TYR")then
            xx(i+5)=xx(i+4)+0.937*cx  !!OH
            yy(i+5)=yy(i+4)+0.937*cy
            zz(i+5)=zz(i+4)+0.937*cz
         endif
         ASSrd(i)=1
         ASSrd(i+1)=1
         ASSrd(i+2)=1
         ASSrd(i+3)=1
      elseif(resd(i).eq."TRP")then
         fbd=1.19/r1
         xx(i)=xx(i-1)+0.63*cx+fbd*ctbcx !CD1
         yy(i)=yy(i-1)+0.63*cy+fbd*ctbcy
         zz(i)=zz(i-1)+0.63*cz+fbd*ctbcz
         xx(i+1)=xx(i-1)+0.448*cx-fbd*ctbcx !!CD2
         yy(i+1)=yy(i-1)+0.448*cy-fbd*ctbcy
         zz(i+1)=zz(i-1)+0.448*cz-fbd*ctbcz
         fbd=0.688/r1
         xx(i+2)=xx(i)+0.819*cx-fbd*ctbcx  !!NE1
         yy(i+2)=yy(i)+0.819*cy-fbd*ctbcy
         zz(i+2)=zz(i)+0.819*cz-fbd*ctbcz
         xx(i+3)=xx(i+1)+0.774*cx+fbd*ctbcx !!CE2
         yy(i+3)=yy(i+1)+0.774*cy+fbd*ctbcy
         zz(i+3)=zz(i+1)+0.774*cz+fbd*ctbcz
         fbd=1.19/r1
         xx(i+4)=xx(i+1)-fbd*ctbcx-0.448*cx !CE3
         yy(i+4)=yy(i+1)-fbd*ctbcy-0.448*cy
         zz(i+4)=zz(i+1)-fbd*ctbcz-0.448*cz 
         xx(i+5)=xx(i+3)-fbd*ctbcx+0.448*cx !CZ2
         yy(i+5)=yy(i+3)-fbd*ctbcy+0.448*cy
         zz(i+5)=zz(i+3)-fbd*ctbcz+0.448*cz      
         xx(i+6)=xx(i+4)-fbd*ctbcx+0.448*cx !CZ3
         yy(i+6)=yy(i+4)-fbd*ctbcy+0.448*cy
         zz(i+6)=zz(i+4)-fbd*ctbcz+0.448*cz      
         xx(i+7)=xx(i+6)+xx(i+3)-xx(i+1)    !CH2
         yy(i+7)=yy(i+6)+yy(i+3)-yy(i+1)
         zz(i+7)=zz(i+6)+zz(i+3)-zz(i+1)  
         do j=1,7
            ASSrd(i+j)=1
         enddo
      elseif(resd(i).eq."HIS")then
         fbd=1.19/r1
         xx(i)=xx(i-1)+0.6*cx+fbd*ctbcx  !!ND1
         yy(i)=yy(i-1)+0.6*cy+fbd*ctbcy
         zz(i)=zz(i-1)+0.6*cz+fbd*ctbcz
         xx(i+1)=xx(i-1)+0.63*cx-fbd*ctbcx !!CD2
         yy(i+1)=yy(i-1)+0.63*cy-fbd*ctbcy
         zz(i+1)=zz(i-1)+0.63*cz-fbd*ctbcz
         fbd=0.688/r1
         xx(i+2)=xx(i-1)+1.35*cx+fbd*ctbcx  !!CE1
         yy(i+2)=yy(i-1)+1.35*cy+fbd*ctbcy
         zz(i+2)=zz(i-1)+1.35*cz+fbd*ctbcz
         xx(i+3)=xx(i-1)+1.41*cx-fbd*ctbcx !!NE2
         yy(i+3)=yy(i-1)+1.41*cy-fbd*ctbcy
         zz(i+3)=zz(i-1)+1.41*cz-fbd*ctbcz
         do j=1,3
            ASSrd(i+j)=1
         enddo
      else
         x=0.5*(xx(ip2)+xx(ip3))
         y=0.5*(yy(ip2)+yy(ip3))
         z=0.5*(zz(ip2)+zz(ip3))
         r1=sqrt((x-ax)*(x-ax)+(y-ay)*(y-ay)+(z-az)*(z-az))
         if(r1>dismin2)then
            fbd=bd/r1
            xx(i)=ax+fbd*(ax-x)
            yy(i)=ay+fbd*(ay-y)
            zz(i)=az+fbd*(az-z)
         else
            ASSrd(i)=2           
            ip3=ipp(1)
            xx(i)=ax+xx(ip2)-xx(ip3)
            yy(i)=ay+yy(ip2)-yy(ip3)
            zz(i)=az+zz(ip2)-zz(ip3)
         endif
      endif
   endif  !!ring finish
endif
end if
!print*,i,atomty(i),resd(i),xx(i),yy(i),zz(i)
!if(xx(i)>990.0)print*,i,atomty(i),resd(i),xx(i)
enddo  !!heavy atom scan finish
!print*,"HEAVY add finish"
end subroutine HEAVY_ADD

!!this subroutine add hydrogen atoms
subroutine HADD
implicit none

!call HEAVY_ADD
i=natom+1
do while(i>2)
   i=i-1
   if(xx(i)<990.or.aty(i)/='H')cycle
   icc=bonda(i,1)  !!bonded heavy atom
   ax=xx(icc)  !!origin point
   ay=yy(icc)
   az=zz(icc)
   bd=bond(i,1)
   ires=ress(i)
   kx0=0

!   bd=bd*0.864

   do j=1,4 !!the connect heavy atom  !!ipx-ip2-icc-H (1,2,3)
      ii=bonda(icc,j)
      if(ii<1)goto 301
      if(xx(ii)>990.or.ii.eq.i.or.aty(ii).eq."H")cycle
      ip2=ii
      kx0=kx0+1
      ipp0(kx0)=ip2
   enddo
301 continue
   ip2=ipp0(1)

      ASSrd(i)=1
!!!get the angle ip2-icc-i
      i1=atc(i)
      i2=atc(icc)
      i3=atc(ip2)
      if(i1>i3)then
         i1=atc(ip2)
         i3=atc(i)
      endif
      angg=ag0(i1,i2,i3)*hpi

      kx=0
      do j=1,4
         ii=bonda(ip2,j)
         if(ii<1.or.ii==icc.or.ATY(ii)=='H'.or.xx(ii)>990)cycle
         ip3=bonda(ip2,j)
         kx=kx+1
         ipp(kx)=ip3
      enddo
      vr0(1)=xx(ip2)-ax
      vr0(2)=yy(ip2)-ay
      vr0(3)=zz(ip2)-az

!if(ca(ires,2).eq.1)then
!   write(*,"I5,2I4,A4,A5,3I5,2f6.3"),i,ires,ca(ires,2),resd(i),atomty(i),icc,ip2,ip3,bd,angg
!endif
!!!rotation axis , cross of vr0 and vr1, first H is transto ip2 
if(atomty(i)(1:2)=='3H')then  !!term     sp3-sp3  trans-, Gauche-, Gauche+
  if(aty(icc).eq."C")bd=bd*0.933  !1.05
            ASSrd(i-1)=1
            ASSrd(i-2)=1
         do k=1,kx
            vr1(k,1)=xx(ipp(k))-xx(ip2)   !!connect heavy atoms vectors
            vr1(k,2)=yy(ipp(k))-yy(ip2)   !!connect heavy atoms vectors
            vr1(k,3)=zz(ipp(k))-zz(ip2)   !!connect heavy atoms vectors
         enddo
         x=xx(ip2)
         y=yy(ip2)
         z=zz(ip2)
         xi=vr1(1,2)*vr0(3)-vr1(1,3)*vr0(2)
         yi=vr1(1,3)*vr0(1)-vr1(1,1)*vr0(3)
         zi=vr1(1,1)*vr0(2)-vr1(1,2)*vr0(1)
!print*,"ccc",x,y,z,xi,yi,zi,ax,ay,az,angg
         call rotate_matrix2(x,y,z,ax,ay,az,xi,yi,zi,angg)  !H on N, each H is trans    
         r1=sqrt((x-ax)**2+(y-ay)**2+(z-az)**2)
         if(r1<0.01.or.r1>5.0)then
          !  print*, "warning CH3-NH3-1",r1,bd,ress2(i),i,atom(i,1)
            r1=sqrt((xx(ip3)-xx(ip2))**2+(yy(ip3)-yy(ip2))**2+(zz(ip3)-zz(ip2))**2)
            if(r1>10.0.or.r1<0.001)then
!               print*,"neighbour position severa wrong CH3-NH3, again"
               xx(i)=ax+(xx(ip2)-xx(ip3))/bond(ip2,2)
               yy(i)=ay+(yy(ip2)-yy(ip3))/bond(ip2,2)
               zz(i)=az+(zz(ip2)-zz(ip3))/bond(ip2,2) 
               ASSrd(i)=2
            else
               fbd=bd/r1
               xx(i)=ax+(xx(ip2)-xx(ip3))*fbd
               yy(i)=ay+(yy(ip2)-yy(ip3))*fbd
               zz(i)=az+(zz(ip2)-zz(ip3))*fbd
            endif
         else
!print*,"ddd",x,y,z,xi,yi,zi,ax,ay,az,r1,bd,angg
            fbd=bd/r1  !!adjust bond length
            xx(i)=ax+fbd*(x-ax)
            yy(i)=ay+fbd*(y-ay)
            zz(i)=az+fbd*(z-az)
         endif
         angg=2.0*pi/3.0
            x=xx(i)
            y=yy(i)
            z=zz(i)
            xi=vr0(1)
            yi=vr0(2)
            zi=vr0(3)
            call rotate_matrix2(x,y,z,ax,ay,az,xi,yi,zi,angg)  !H on N, each H is trans    
            r1=sqrt((x-ax)**2+(y-ay)**2+(z-az)**2)
            if(r1<0.01.or.r1>100.0)then
            !   print*, "warning CH3,NH3-2",r1,bd,ress2(i),i,atom(i,1)           
               xx(i-1)=ax+cos(109.5/180*pi)*(xx(i)-ax)
               yy(i-1)=ay+sin(109.5/180*pi)*(yy(i)-ay)
               zz(i-1)=az 
               ASSrd(i-1)=2
            else
               fbd=bd/r1  !!adjust bond length
               xx(i-1)=ax+fbd*(x-ax)
               yy(i-1)=ay+fbd*(y-ay)
               zz(i-1)=az+fbd*(z-az)  
            endif
            x=xx(i)
            y=yy(i)
            z=zz(i)
            call rotate_matrix2(x,y,z,ax,ay,az,xi,yi,zi,-angg)  !H on N, each H is trans  
            r1=sqrt((x-ax)**2+(y-ay)**2+(z-az)**2)
            if(r1<0.01.or.r1>10.0)then
!               print*, "warning CH3,NH3-2",r1,bd,ress2(i),i,atom(i,1)           
               xx(i-2)=ax+cos(109.5/180*pi)*(xx(i)-ax)
               yy(i-2)=ay+sin(-109.5/180*pi)*(yy(i)-ay)
               zz(i-2)=az 
               ASSrd(i-1)=2
            else
               fbd=bd/r1  !!adjust bond length
               xx(i-2)=ax+fbd*(x-ax)
               yy(i-2)=ay+fbd*(y-ay)
               zz(i-2)=az+fbd*(z-az)   
            endif
            i=i-2
!!!!sp3 -term finish ****************************************************************
      elseif(atomty(i)(1:2)=='2H')then  !!in term or middle
     !    bd=bd*1.08
         if(aty(icc).eq."C")bd=bd*0.933
         ASSrd(i-1)=1
         if(ATY(icc)=='N')then   !!in term, sp2   
 !           if(kx==2)then    !!sp2-sp2 term
               if(ATY(ipp(1)).eq.'C')then   !!N=O -C
                  ip3=ipp(2)
               else
                  ip3=ipp(1)
               endif  !!the O
            !   if(resd(i).eq."ARG")then  !!N-C-N-H
            !      angg=1.02*angg
            !   else  !!O=C-N-H
            !      angg=0.98*angg
            !   endif
               vr1(1,1)=xx(ip3)-xx(ip2)
               vr1(1,2)=yy(ip3)-yy(ip2)
               vr1(1,3)=zz(ip3)-zz(ip2)
               x=xx(ip2)
               y=yy(ip2)
               z=zz(ip2)
               xi=vr1(1,2)*vr0(3)-vr1(1,3)*vr0(2)
               yi=vr1(1,3)*vr0(1)-vr1(1,1)*vr0(3)
               zi=vr1(1,1)*vr0(2)-vr1(1,2)*vr0(1)
               call rotate_matrix2(x,y,z,ax,ay,az,xi,yi,zi,angg)  !H on N, each H is trans
               r1=sqrt((x-ax)**2+(y-ay)**2+(z-az)**2)
               if(r1<0.01.or.r1>5.0)then
                  ASSrd(i)=2
                  r1=sqrt((xx(ip3)-xx(ip2))**2+(yy(ip3)-yy(ip2))**2+(zz(ip3)-zz(ip2))**2)
                  if(r1>10.0.or.r1<0.001)then
!                     print*,"neighbour bond position severa wrong NH2 again"
                     xx(i)=ax+(xx(ip2)-xx(ip3))*0.6
                     yy(i)=ay+(yy(ip2)-yy(ip3))*0.6
                     zz(i)=az+(zz(ip2)-zz(ip3))*0.6  
                  else
                     fbd=bd/r1
                     xx(i)=ax+(xx(ip2)-xx(ip3))*fbd
                     yy(i)=ay+(yy(ip2)-yy(ip3))*fbd
                     zz(i)=az+(zz(ip2)-zz(ip3))*fbd       
                  endif
               else
                  fbd=bd/r1  !!adjust bond length
                  xx(i)=ax+fbd*(x-ax)
                  yy(i)=ay+fbd*(y-ay)
                  zz(i)=az+fbd*(z-az)  
               endif
               x=xx(ip2)
               y=yy(ip2)
               z=zz(ip2)
               call rotate_matrix2(x,y,z,ax,ay,az,xi,yi,zi,-angg)  !H on N, each H is trans              
               r1=sqrt((x-ax)**2+(y-ay)**2+(z-az)**2)
               if(r1<0.01.or.r1>5.0)then
                  r1=sqrt((xx(ip3)-xx(ip2))**2+(yy(ip3)-yy(ip2))**2+(zz(ip3)-zz(ip2))**2)
                  if(r1>5.0.or.r1<0.001)then
                     xx(i-1)=ax+(xx(ip2)-xx(ip3))*0.6
                     yy(i-1)=ay+(yy(ip2)-yy(ip3))*0.6
                     zz(i-1)=az+(zz(ip2)-zz(ip3))*0.6 
                     ASSrd(i-1)=2
                     print*,"severa wrong NH2-2",i-1,xx(i-1),yy(i-1)
                  else
                     fbd=bd/r1
                     xx(i-1)=ax+(xx(ip3)-xx(ip2))*fbd
                     yy(i-1)=ay+(yy(ip3)-yy(ip2))*fbd
                     zz(i-1)=az+(zz(ip3)-zz(ip2))*fbd  
                  endif
               else
                  fbd=bd/r1  !!adjust bond length
                  xx(i-1)=ax+fbd*(x-ax)
                  yy(i-1)=ay+fbd*(y-ay)
                  zz(i-1)=az+fbd*(z-az)
               endif
               if(ca(i,2)==8.and.ress2(i)==1)then   !!PRO N-term
               xi=vr0(1)
               yi=vr0(2)
               zi=vr0(3)
               do k=-1,0
                  x=xx(i+k)
                  y=yy(i+k)
                  z=zz(i+k)
                  call rotate_matrix2(x,y,z,ax,ay,az,xi,yi,zi,0.5*pi)  !H on N, each H is trans  
                  xx(i+k)=x
                  yy(i+k)=y
                  zz(i+k)=z
               enddo
            endif       
         else    !!!C inter connected, not in term, rotate first H on of ip2 around icc-ip4
            if(ipp0(2)==ip2)then
               ip4=ipp0(1)
               if(abs(icc-ip4)>20)print*,"ip4 wrong!!!"
            else
               ip4=ipp0(2)
            endif
            angg=1.1*angg  !!!120/110 degree
         do ii=1,2
            if(ii==2)then
               i2=ip2
               ip2=ip4
               ip4=i2
               angg=-angg
            endif               
            x=xx(ip2)
            y=yy(ip2)
            z=zz(ip2)
            xi=xx(ip4)-ax
            yi=yy(ip4)-ay
            zi=zz(ip4)-az
            call rotate_matrix2(x,y,z,ax,ay,az,xi,yi,zi,angg)  !H on C, each H is trans   
            r1=sqrt((x-ax)**2+(y-ay)**2+(z-az)**2)
            if(r1<0.01.or.r1>5.0)then
               r1=sqrt((xx(ip3)-xx(ip2))**2+(yy(ip3)-yy(ip2))**2+(zz(ip3)-zz(ip2))**2)
               if(r1>10.0.or.r1<0.001)then
!                  print*,"neighbour bond position severa wrong CH2-1 again"
                  xx(i)=ax+(xx(ip2)-xx(ip3))/bond(ip2,2)
                  yy(i)=ay+(yy(ip2)-yy(ip3))/bond(ip2,2)
                  zz(i)=az+(zz(ip2)-zz(ip3))/bond(ip2,2) 
                  ASSrd(i)=2
               else
                  fbd=bd/r1
                  xx(i)=ax+(xx(ip3)-xx(ip2))*fbd
                  yy(i)=ay+(yy(ip3)-yy(ip2))*fbd
                  zz(i)=az+(zz(ip3)-zz(ip2))*fbd  
               endif
            else
               fbd=bd/r1  !!adjust bond length
               if(ii<2)then
               xx(i)=ax+fbd*(x-ax)
               yy(i)=ay+fbd*(y-ay)
               zz(i)=az+fbd*(z-az)              
               else
                  xx(i)=0.5*(xx(i)+ax+fbd*(x-ax))
                  yy(i)=0.5*(yy(i)+ay+fbd*(y-ay))
                  zz(i)=0.5*(zz(i)+az+fbd*(z-az))
               endif
            endif
  
            x=xx(ip2)
            y=yy(ip2)
            z=zz(ip2)
            call rotate_matrix2(x,y,z,ax,ay,az,xi,yi,zi,-angg)  !H on C, each H is trans   
            r1=sqrt((x-ax)**2+(y-ay)**2+(z-az)**2)
            if(r1<0.01.or.r1>5.0)then
               r1=sqrt((xx(ip3)-xx(ip2))**2+(yy(ip3)-yy(ip2))**2+(zz(ip3)-zz(ip2))**2)
               if(r1>10.0.or.r1<0.001)then
                  print*,"neighbour bond position severa wrong CH2-2",i,xx(i)
                  xx(i)=ax+(xx(ip2)-xx(ip3))/bond(ip2,2)
                  yy(i)=ay+(yy(ip2)-yy(ip3))/bond(ip2,2)
                  zz(i)=az+(zz(ip2)-zz(ip3))/bond(ip2,2) 
                  ASSrd(i)=2
               else
                  fbd=bd/r1
                  xx(i-1)=ax+(xx(ip2)-xx(ip3))*fbd
                  yy(i-1)=ay+(yy(ip2)-yy(ip3))*fbd
                  zz(i-1)=az+(zz(ip2)-zz(ip3))*fbd 
               endif
            else
               fbd=bd/r1  !!adjust bond length
               if(ii<2)then
               xx(i-1)=ax+fbd*(x-ax)
               yy(i-1)=ay+fbd*(y-ay)
               zz(i-1)=az+fbd*(z-az)   
               else
                  xx(i-1)=0.5*(xx(i-1)+ax+fbd*(x-ax))
                  yy(i-1)=0.5*(yy(i-1)+ay+fbd*(y-ay))
                  zz(i-1)=0.5*(zz(i-1)+az+fbd*(z-az))
               endif
            endif
         enddo  !!!ii=1,2
         endif   !!  2H finished
         i=i-1
      else   !!1H case, sp2 has been assign, O-H and S-H also done trans-; only for sp3 C case
         if(kx0==3)then   !!sp3 case
            bd=bd*0.945
            x=(xx(ipp0(1))+xx(ipp0(2))+xx(ipp0(3)))/3.0
            y=(yy(ipp0(1))+yy(ipp0(2))+yy(ipp0(3)))/3.0
            z=(zz(ipp0(1))+zz(ipp0(2))+zz(ipp0(3)))/3.0
            r1=sqrt((x-ax)**2+(y-ay)**2+(z-az)**2)
            if(r1<0.01.or.r1>10.0)then
!               print*, "warning H1-sp3",ress2(i),atom(i,1),i,r1
               ASSrd(i)=2
               xx(i)=ax+0.3
               yy(i)=ay+0.6
               zz(i)=az+0.7
            else
               fbd=bd/r1  !!adjust bond length
               xx(i)=ax+fbd*(ax-x)
               yy(i)=ay+fbd*(ay-y)
               zz(i)=az+fbd*(az-z)
            endif
         elseif(kx0==2)then   !!sp2, aromatic ring H, H-N
            bd=bd*0.95
            x=(xx(ipp0(1))+xx(ipp0(2)))*0.5
            y=(yy(ipp0(1))+yy(ipp0(2)))*0.5
            z=(zz(ipp0(1))+zz(ipp0(2)))*0.5
            r1=sqrt((x-ax)**2+(y-ay)**2+(z-az)**2)
            if(r1<0.01.or.r1>5.0)then
               ASSrd(i)=2
               xx(i)=ax+0.3
               yy(i)=ay+0.6
               zz(i)=az+0.7
            else
               fbd=bd/r1  !!adjust bond length
               xx(i)=ax+fbd*(ax-x)
               yy(i)=ay+fbd*(ay-y)
               zz(i)=az+fbd*(az-z)
            endif
         elseif(kx0==1)then   !!H-O,H-S
            bd=bd*0.94
            vr1(1,1)=xx(ipp(1))-xx(ip2)
            vr1(1,2)=yy(ipp(1))-yy(ip2)
            vr1(1,3)=zz(ipp(1))-zz(ip2)
            x=xx(ip2)
            y=yy(ip2)
            z=zz(ip2)
            xi=vr1(1,2)*vr0(3)-vr1(1,3)*vr0(2)
            yi=vr1(1,3)*vr0(1)-vr1(1,1)*vr0(3)
            zi=vr1(1,1)*vr0(2)-vr1(1,2)*vr0(1)
            angg=angg
            call rotate_matrix2(x,y,z,ax,ay,az,xi,yi,zi,angg)  !H on N, each H is trans            
            r1=sqrt((x-ax)**2+(y-ay)**2+(z-az)**2)
            if(r1<0.01.or.r1>5.0)then
!               print*, "warning O-H",ress2(i),atom(i,1),i,r1
               ASSrd(i)=2
               xx(i)=ax+0.3
               yy(i)=ay+0.6
               zz(i)=az+0.7  
            else
               fbd=bd/r1  !!adjust bond length
               xx(i)=ax+fbd*(x-ax)
               yy(i)=ay+fbd*(y-ay)
               zz(i)=az+fbd*(z-az)
            endif
         endif
      endif   !!3H
   enddo  !!kk<NHHig  
call AD_1H  !!!this incluse Imerge(1)
410 continue
    end subroutine HADD
!!this subroutine to adjust 1H case 
subroutine AD_1H
implicit none
integer icc,i,ii,ip2,ind,np0,ir0,ir1
real arrin(NMAX)
real bd,dx,dy,dz,rf,r,vx1,vy1,vz1


  do i=1,natom
 !    if(atomty(i).eq." H  ".or.atomty(i).eq." HG ".or.atomty(i).eq." HG1".or.atomty(i).eq." HH ")then
    ! if(atomty(i).eq." HG ".or.atomty(i).eq." HG1".or.atomty(i).eq." HH ")then
     if(aty(i).eq."H")then
        icc=bonda(i,1)  !!bonded heavy atom
        if(.not.(aty(icc).eq."O".or.aty(icc).eq."S"))cycle
  !      print*,"ADD1H00",i,atomty(i)
        bd=bond(i,1)
        ax=xx(icc)  !!origin point
        ay=yy(icc)
        az=zz(icc)
        ip2=bonda(icc,1) !ip2-icc-iH
        np0=0 !1record the number of atoms in contact
        j=0
        ir0=ress(i)
        do while(j<natom)
           j=j+1
           ir1=ress(j)
           if(ir1.eq.ir0)cycle
           !           if(j.eq.i.or.j.eq.icc.or.j.eq.ip2)cycle
           r=(xx(i)-xx(j))**2+(yy(i)-yy(j))**2+(zz(i)-zz(j))**2
         !  print*,i,j,r,ir1,Nres
           if(r>144.0)then
              if(ir1<Nres)then !!12A no contact with this residue
                 j=ca(ir1+1,1)-2
         !        print*,">>",i,j,r,np0
                 cycle
              else
                 goto 231
              endif
           endif
           if(r>0.01.and.r<25.0)then
              np0=np0+1
              HH1(np0)=j
              rHH1(np0)=r !!use aty(j) to judge whether its overlap
           endif
        enddo
231     continue
        if(np0<1)cycle
 !       print*,"nop",i,np0,atomty(i)
        do j=1,np0
           arrin(j)=rHH1(j)
        enddo
        if(np0>1)then
           call sortindex(np0,arrin,indx) !!increaing order
        else
           indx(1)=1           
        endif
!!decide the vecoter summ  
        x=0
        y=0
        z=0
        do j=1,np0
!!four cases, Hbond yes/not, attractive/repulsive
           ind=indx(j)
           ip=HH1(ind)  !!atom id
           if(rHH1(ind)<0.01)cycle
           if(aty(ip).eq."C".or.aty(ip).eq."S")then
              r=9.0 !3.0^2
           elseif(aty(ip).eq."N")then
              r=8.12 !2.85^2
           elseif(aty(ip).eq."H")then
              r=4.0
           elseif(aty(ip).eq."O")then
              r=7.67 !2.77^2
           endif
           if(rHH1(ind)<r.and.aty(ip).ne."O")then !!repulsion and Hbond
              MH1=-1
           elseif(aty(ip).eq."O")then
              MH1=7
           elseif(aty(ip).eq."N")then
              MH1=1
           else
              MH1=0              
           endif      
           vx1=(xx(ip)-xx(i))/rHH1(ind)
           vy1=(yy(ip)-yy(i))/rHH1(ind)
           vz1=(zz(ip)-zz(i))/rHH1(ind)
           x=x+vx1*MH1
           y=y+vy1*MH1
           z=z+vz1*MH1
        enddo  !!the vector from current position
        r=sqrt(x*x+y*y+z*z)
        xi=xx(ip2)-ax
        yi=yy(ip2)-ay
        zi=zz(ip2)-az
        if(r<0.01)r=0.01
        x=x/r
        y=y/r
        z=z/r
        if(aty(icc).eq."O")then
           rf=(x*(xx(i)-xx(icc))+y*(yy(i)-yy(icc))+z*(zz(i)-zz(icc)))/bond(i,1)/0.956 !cos(0.1)
        else !S
           rf=(x*(xx(i)-xx(icc))+y*(yy(i)-yy(icc))+z*(zz(i)-zz(icc)))/bond(i,1)/0.996 
        endif
        if(abs(rf)>=1.0)cycle
        angg=acos(rf)
        x=xx(i)
        y=yy(i)
        z=zz(i)
        call rotate_matrix2(x,y,z,ax,ay,az,xi,yi,zi,angg)  !H on N, each H is trans   
   !     r=sqrt((x-xx(i))**2+(y-yy(i))**2+(z-zz(i))**2)
!        r=sqrt((x-xx(icc))**2+(y-yy(icc))**2+(z-zz(icc))**2)
!write(*,"A10,A5,I5,f6.3,7f8.3"),"rotation angg",atomty(i),i,angg,xx(i),yy(i),zz(i),x,y,z,r  
        xx(i)=x
        yy(i)=y
        zz(i)=z
     endif !!H
  end do !!all spH finish
end subroutine AD_1H

!this sub for sort, output in increase series
!     puts into a the permutation vector which sorts v into increasing order. 
SUBROUTINE sort(RA,N)
  implicit none
      real RA(N)
      real rra
      integer N,ir,L,i,j
      L=N/2+1
      IR=N
10    IF (L.GT.1) THEN
        L=L-1
        RRA=RA(L)
      ELSE
        RRA=RA(IR)
        RA(IR)=RA(1)
        IR=IR-1
        IF (IR.EQ.1) THEN
          RA(1)=RRA
          RETURN
        END IF
      END IF
      I=L
      J=L+L
20    IF (J.LE.IR) THEN
        IF (J.LT.IR) THEN
          IF (RA(J).LT.RA(J+1)) J=J+1
        END IF
        IF (RRA.LT.RA(J)) THEN
          RA(I)=RA(J)
          I=J
          J=J+J
        ELSE
          J=IR+1
        END IF
        GO TO 20
      END IF
      RA(I)=RRA
      GO TO 10
END subroutine sort 
!!this subroutine to sort array a and return accerating index after sorting
!!Note the input array will change value
SUBROUTINE sortindex(n,arrin,indx)
   implicit none
   integer n
   integer indx(n)
   real arrin(n)
   integer i,j,k,ir,indxt
   real q
   do j=1,n
      indx(j)=j
   end do
   k=n/2+1
   ir=n
   do
      if(k>1)then
         k=k-1
         indxt=indx(k)
         q=arrin(indxt)
      else
         indxt=indx(ir)
         q=arrin(indxt)
         indx(ir)=indx(1)
         ir=ir-1
         if(ir==1) then
            indx(1)=indxt
            return
         endif
      endif
      i=k
      j=k+k
      do while(j<=ir) 
         if(j<ir) then
            if(arrin(indx(j))<arrin(indx(j+1)))j=j+1
         endif
         if(q<arrin(indx(j))) then
            indx(i)=indx(j)
            i=j
            j=j+j
         else
            j=ir+1
         endif
      end do
      indx(i)=indxt
   end do
 END SUBROUTINE sortindex


  subroutine rotate_matrix2(x,y,z,ax,ay,az,xi,yi,zi,an) !rotate (x,y,z) with origin (ax,ay,az) around axis (xi,yi,zi) an
    implicit none
    real x,y,z,ax,ay,az,xi,yi,zi,an
    real r,r12,r23,r13,rcor
    real xr,yr,zr
    real x0,y0,z0    !!only use in test
    integer i,j,k
r=sqrt(xi*xi+yi*yi+zi*zi)
if(r<0.1)goto 101
x0=x
y0=y
x0=z
xi=xi/r
yi=yi/r
zi=zi/r
r12=xi*xi+yi*yi
r23=yi*yi+zi*zi
r13=xi*xi+zi*zi
rcor=xi*x+yi*y+zi*z
xr=ax*r23+xi*(-ay*yi-az*zi+rcor)+((x-ax)*r23+xi*(ay*yi+az*zi-yi*y-zi*z))*cos(an)+(ay*zi-az*yi-zi*y+yi*z)*sin(an)
yr=ay*r13+yi*(-ax*xi-az*zi+rcor)+((y-ay)*r13+yi*(ax*xi+az*zi-xi*x-zi*z))*cos(an)+(-ax*zi+az*xi+zi*x-xi*z)*sin(an)
zr=az*r12+zi*(-ax*xi-ay*yi+rcor)+((z-az)*r12+zi*(ax*xi+ay*yi-xi*x-yi*y))*cos(an)+(ax*yi-ay*xi-yi*x+xi*y)*sin(an)
x=xr
y=yr
z=zr
!if(abs(x)+abs(y)+abs(z)<0.001)then
!   print*,"warning",xi,yi,zi,r,x,y,z,an,x0,y0,z0
!endif
101 continue
  end subroutine rotate_matrix2

!!this subroutine get the residue series, Iresin (public value)
subroutine getresin
character*3 resn
integer ir,nat,ipca
!!get the atom name list
iatom=0
ress2=0
do ir=1,Nres
   resn=resd2(ir)
!!get the residue index
   iresin=0
   do j=1,20
      if(resn.eq.afn(j))then
         iresin=j
         goto 101
      endif
   enddo
101 continue
   if(iresin<1)then !!additional residue
      if(resn.eq."HSD".or.resn.eq."HSE".or.resn.eq."HSP".or.resn.eq."HIE")then
         iresin=17 !!HIS
      elseif(resn.eq."MSE")then
         iresin=9 !!MET
      else
         iresin=1 !GLY
         print*,"WARNING on reside abnormal",ir,resn
      endif
   endif
   ca(ir,1)=iatom+2
   ca(ir,2)=iresin
   if(ir==1)then
      do j=1,hra(iresin)
         iatom=iatom+1
         atomty(iatom)=atomname(iresin,j)
         atc(iatom)=atomct(iresin,j)
         ress2(iatom)=ir
         resd(iatom)=afn(iresin)
      enddo
      atomty(iatom+1)="1H  "
      atomty(iatom+2)="2H  "
      atc(iatom+1)=20
      atc(iatom+2)=20
      ress2(iatom+1)=ir
      ress2(iatom+2)=ir
      resd(iatom+1)=afn(iresin)
      resd(iatom+2)=afn(iresin)
      if(iresin==8)then
         atomty(iatom+3)=" HA "
         atc(iatom+3)=19
         atc(1)=33
         ress2(iatom+3)=ir
         resd(iatom+3)=afn(iresin)
         iatom=iatom+3
      else
         atomty(iatom+3)="3H  "
         atc(iatom+3)=20
         atc(1)=12
         ress2(iatom+3)=ir
         resd(iatom+3)=afn(iresin)
         iatom=iatom+3
      endif
      do j=hra(iresin)+2,ra(iresin)
         iatom=iatom+1
         atomty(iatom)=atomname(iresin,j)
         atc(iatom)=atomct(iresin,j)
         resd(iatom)=afn(iresin)
         ress2(iatom)=ir
      enddo
   elseif(ir==Nres)then
      do j=1,hra(iresin)
         iatom=iatom+1
         atomty(iatom)=atomname(iresin,j)
         atc(iatom)=atomct(iresin,j)
         resd(iatom)=afn(iresin)
         ress2(iatom)=ir
      enddo
      atc(ca(Nres,1)+1)=5 !!change C to CC
      iatom=iatom+1
      atomty(iatom)=" OXT"
      atc(iatom)=27
      resd(iatom)=afn(iresin)
      ress2(iatom)=ir
      do j=hra(iresin)+1,ra(iresin)
         iatom=iatom+1
         atomty(iatom)=atomname(iresin,j)
         atc(iatom)=atomct(iresin,j)
         resd(iatom)=afn(iresin)
         ress2(iatom)=ir
      enddo      
   else
      do j=1,ra(iresin)
         iatom=iatom+1
         atomty(iatom)=atomname(iresin,j)
         atc(iatom)=atomct(iresin,j)
         resd(iatom)=afn(iresin)
         ress2(iatom)=ir
      enddo
   endif
!print*,ir,iatom,iresin,resd(iatom)
end do
Natom=iatom  !!!all the atomname assign finish
do i=1,Natom
   aty(i)=atomty(i)(2:2)
enddo

!!assign the bond network
   iresin=ca(1,2)
   bdc=0
   nat=ra(iresin)
   Nhy=hra(iresin)
   call Assbonda
   do iatom=1,Nhy
      do j=1,4
         if(bdc(iatom,j)==0)cycle 
 !        if(bdc(iatom,j)+iatom>Nhy)then
         if(bdc(iatom,j)>3)then
            bonda(iatom,j)=iatom+bdc(iatom,j)+2
         else
            bonda(iatom,j)=iatom+bdc(iatom,j)
         endif
      enddo
   enddo
   bonda(1,3)=Nhy+1
   bonda(1,4)=Nhy+2
   bonda(iatom,1)=1
   bonda(iatom+1,1)=1
   iatom=iatom+1
   do i=Nhy+1,nat
      iatom=iatom+1
      bonda(iatom,1)=iatom+bdc(i,1)-2
   enddo   
   bonda(3,2)=ca(2,1)-1
   if(Iresin.eq.8)then !!PRO
      bonda(1,2)=Nhy
      
      
   endif
!!!N_term finish

   do k=2,Nres-1
      iresin=ca(k,2)
      bdc=0
      nat=ra(iresin)
      Nhy=hra(iresin)
      call Assbonda
      do i=1,nat
         iatom=iatom+1
         do j=1,4
            if(bdc(i,j)==0)cycle 
            bonda(iatom,j)=iatom+bdc(i,j)
         enddo
      enddo
      ipca=ca(k,1)
      bonda(ipca-1,3)=ca(k-1,1)+1
      bonda(ipca+1,2)=ca(k+1,1)-1
   enddo
!C-Term
   iresin=ca(Nres,2)
   bdc=0
   nat=ra(iresin)
   Nhy=hra(iresin)
   ipca=ca(Nres,1)
   call Assbonda
   do i=1,Nhy
      iatom=iatom+1
      do j=1,4
         if(bdc(i,j)==0)cycle
         if(bdc(i,j)>3)then
            bonda(iatom,j)=iatom+bdc(i,j)+1
         else
            bonda(iatom,j)=iatom+bdc(i,j)
         endif
      enddo
   enddo
   bonda(iatom+1,1)=ipca+1
   bonda(ipca+1,2)=iatom+1
   iatom=iatom+1
   do i=Nhy+1,nat
      iatom=iatom+1
      bonda(iatom,1)=iatom+bdc(i,1)-1
   enddo
   bonda(ipca-1,3)=ca(Nres-1,1)+1
   if(iresin.eq.8)then
      bonda(ca(Nres,1)-1,2)=bonda(ca(Nres,1)-1,2)-1
   endif
333 continue
!bond vector assign finish
do i=1,0 !Natom
   nat=bonda(i,2)
   if(nat>=1)then
      write(*,1104),i,atomty(i),resd(i),bonda(i,1),bonda(i,2),bonda(i,3),bonda(i,4),atomty(bonda(i,1)),atomty(nat),atomty(bonda(i,3)),atomty(bonda(i,4))
   else
      write(*,1105),i,atomty(i),resd(i),bonda(i,1),bonda(i,2),bonda(i,3),bonda(i,4),atomty(bonda(i,1)),nat
   endif
enddo
1104 format(I4,A5,A4,4I5,4A5)
1105 format(I4,A5,A4,4I5,A5,I3)

!!assign the bondlength 
call bondp
   do i=1,natom    
      i1=atc(i)
      do j=1,4
         if(bonda(i,j)<1)cycle
         i2=atc(bonda(i,j))
         if(i1<=i2)then
            bond(i,j)=bd0(i1,i2)
         else
            bond(i,j)=bd0(i2,i1)
         endif
         if(bond(i,j)<0.5)then
            write(*,1106),"bondm",i,bonda(i,j),i1,i2,atomty(i),atomty(bonda(i,j)),bd0(i1,i2),bd0(i2,i1)
         endif
      enddo
   enddo
1106 format(A6,4I5,2A5,2f8.3)
!!assign angle ///********************//////////////
call angp
do i=1,natom  !!check whether all are in position
!   if(aty(i).ne."H")cycle
   kx0=0
   ip2=0
   icc=bonda(i,1)
   do j=1,4 !!the connect heavy atom  !!ipx-ip2-icc-H (1,2,3)
      ii=bonda(icc,j)
      if(ii<1)goto 301
      if(xx(ii)>990.or.ii.eq.i.or.aty(ii).eq."H")cycle
      ip2=ii
      kx0=kx0+1
      ipp0(kx0)=ip2
   enddo
301 continue
   if(ip2<1)goto 302
   i1=atc(i)
   i2=atc(icc)
   i3=atc(ip2)
   if(i1>i3)then
      i1=atc(ip2)
      i3=atc(i)
   endif
   angg=ag0(i1,i2,i3)
   if(angg<1.0)then
      write(*,1107),"anm",i,icc,ip2,atomty(i),atomty(icc),atomty(ip2),i1,i2,i3,atomcontype(i1),atomcontype(i2),atomcontype(i3),bonda(icc,1),bonda(icc,2),bonda(icc,3),bonda(icc,4)
   endif
302 continue
enddo
1107 format(A6,3I5,3A5,3I3,3A5,4I5)

end subroutine getresin


!!this subroutine assign the bond network
!!which atoms are bonded
subroutine assbonda
implicit none
integer ig,nratom  !!residue type

!!seperate backbone and sidechain
!!the internal difference
   Nhy=hra(iresin)
!print*,iresin,Nhy
   bdc(1,1)=1
   bdc(1,2)=Nhy !1,3 to C
   bdc(2,1)=3
   bdc(2,2)=-1
   bdc(2,3)=1
   bdc(2,4)=Nhy
   bdc(3,1)=-1
   bdc(3,3)=1
!   bdc(3,4)=1 !bdc4 to N !!for backbone, only GLY and PRO special
   bdc(4,1)=-1
!   bdc(4,2)=-1
   bdc(Nhy+1,1)=-Nhy !H
   bdc(Nhy+2,1)=-Nhy !HA

   bdc(5,1)=-3 !CB-Ca
   bdc(5,2)=1
   bdc(5,3)=Nhy-1
   bdc(5,4)=Nhy-2
   bdc(Nhy+3,1)=-Nhy+2
   bdc(Nhy+4,1)=-Nhy+1
if(iresin.eq.1)then  !GLY
   bdc(2,1)=Nhy
   bdc(2,4)=Nhy+1
   bdc(Nhy+1,1)=-Nhy
   bdc(5,2)=0
   bdc(5,3)=0
   bdc(5,4)=0
   bdc(nhy+2,1)=-Nhy
   bdc(nhy+3,1)=-Nhy-1
elseif(iresin.eq.2)then !ALA
   bdc(Nhy,2)=3
   bdc(Nhy,3)=4
   bdc(Nhy,4)=5
   bdc(Nhy+3,1)=-Nhy+2
   bdc(Nhy+4,1)=-Nhy+1
   bdc(Nhy+5,1)=-Nhy
elseif(iresin.eq.3.or.iresin.eq.4)then !SER and CYS
   bdc(6,1)=-1
   bdc(6,2)=Nhy-1
   bdc(Nhy+5,1)=-Nhy+1
elseif(iresin.eq.5)then !VAL
   bdc(5,3)=2
   bdc(6,1)=-1
   bdc(6,2)=Nhy-2
   bdc(6,3)=Nhy-1
   bdc(6,3)=Nhy
   bdc(7,1)=-2
   bdc(7,2)=Nhy
   bdc(7,3)=Nhy+1
   bdc(7,4)=Nhy+2
   bdc(Nhy+4,1)=-Nhy+2
   bdc(Nhy+5,1)=-Nhy+1
   bdc(Nhy+6,1)=-Nhy
   bdc(Nhy+7,1)=-Nhy
   bdc(Nhy+8,1)=-Nhy-1
   bdc(Nhy+9,1)=-Nhy-2
elseif(iresin.eq.6)then !THR
   bdc(5,2)=1
   bdc(5,3)=2
   bdc(6,1)=-1
   bdc(6,2)=Nhy-2
   bdc(7,1)=-2
   bdc(7,2)=Nhy-2
   bdc(7,3)=Nhy-1
   bdc(7,4)=Nhy   
   bdc(Nhy+4,1)=-Nhy+2
   bdc(Nhy+5,1)=-Nhy+2
   bdc(Nhy+6,1)=-Nhy+1
   bdc(Nhy+7,1)=-Nhy
elseif(iresin.eq.7)then !ILE
   bdc(5,3)=2
   bdc(6,1)=-1
   bdc(6,2)=2
   bdc(6,3)=Nhy-2
   bdc(6,4)=Nhy-1
   bdc(7,1)=-2
   bdc(7,2)=Nhy-1
   bdc(7,3)=Nhy
   bdc(7,4)=Nhy+1
   bdc(8,1)=-2
   bdc(8,2)=Nhy+1
   bdc(8,3)=Nhy+2
   bdc(8,4)=Nhy+3
   bdc(Nhy+4,1)=-Nhy+2
   bdc(Nhy+5,1)=-Nhy+1
   bdc(Nhy+6,1)=-Nhy+1
   bdc(Nhy+7,1)=-Nhy
   bdc(Nhy+8,1)=-Nhy-1
   bdc(Nhy+9,1)=-Nhy-1
   bdc(Nhy+10,1)=-Nhy-2
   bdc(Nhy+11,1)=-Nhy-3
elseif(iresin.eq.8)then !PRO
   bdc(1,2)=Nhy-1
   bdc(2,4)=Nhy-1
   bdc(5,2)=1
   bdc(5,3)=Nhy-3
   bdc(6,1)=-1
   bdc(6,2)=1
   bdc(6,3)=Nhy-2
   bdc(6,4)=Nhy-1
   bdc(7,1)=-1
   bdc(7,2)=-Nhy+1
   bdc(7,3)=Nhy-1
   bdc(7,4)=Nhy
   bdc(Nhy+1,1)=-Nhy+1
   bdc(Nhy+2,1)=-Nhy+3
   bdc(Nhy+4,1)=-Nhy+2
   bdc(Nhy+5,1)=-Nhy+1
   bdc(Nhy+6,1)=-Nhy+1
   bdc(Nhy+7,1)=-Nhy
elseif(iresin.eq.9)then !MET
   bdc(6,1)=-1
   bdc(6,2)=1
   bdc(6,3)=Nhy-1
   bdc(6,4)=Nhy
   bdc(7,1)=-1
   bdc(7,2)=1
   bdc(8,1)=-1
   bdc(8,2)=Nhy-1
   bdc(8,3)=Nhy
   bdc(8,4)=Nhy+1
   bdc(Nhy+5,1)=-Nhy+1
   bdc(Nhy+6,1)=-Nhy
   bdc(Nhy+7,1)=-Nhy+1
   bdc(Nhy+8,1)=-Nhy
   bdc(Nhy+9,1)=-Nhy-1
elseif(iresin.eq.10.or.iresin.eq.11)then !ASP and ASN
   bdc(6,1)=-1
   bdc(6,2)=1
   bdc(6,3)=2
   bdc(7,1)=-1
   bdc(8,1)=-2
   if(iresin.eq.11)then !ASN
      bdc(8,2)=Nhy-3
      bdc(8,3)=Nhy-2
      bdc(Nhy+5,1)=-Nhy+3
      bdc(Nhy+6,1)=-Nhy+2
   endif
elseif(iresin.eq.12)then !LEU
   bdc(6,1)=-1
   bdc(6,2)=1
   bdc(6,3)=2
   bdc(6,4)=Nhy-1
   bdc(7,1)=-1
   bdc(7,2)=Nhy-1
   bdc(7,3)=Nhy
   bdc(7,4)=Nhy+1
   bdc(8,1)=-2
   bdc(8,2)=Nhy+1
   bdc(8,3)=Nhy+2
   bdc(8,4)=Nhy+3
   bdc(Nhy+5,1)=-Nhy+1
   bdc(Nhy+6,1)=-Nhy+1
   bdc(Nhy+7,1)=-Nhy
   bdc(Nhy+8,1)=-Nhy-1
   bdc(Nhy+9,1)=-Nhy-1
   bdc(Nhy+10,1)=-Nhy-2
   bdc(Nhy+11,1)=-Nhy-3
elseif(Iresin.eq.13)then !LYS
   bdc(6,1)=-1
   bdc(6,2)=1
   bdc(6,3)=Nhy-1
   bdc(6,4)=Nhy
   bdc(7,1)=-1
   bdc(7,2)=1
   bdc(7,3)=Nhy
   bdc(7,4)=Nhy+1
   bdc(8,1)=-1
   bdc(8,2)=1
   bdc(8,3)=Nhy+1
   bdc(8,4)=Nhy+2
   bdc(9,1)=-1
   bdc(9,2)=Nhy+2
   bdc(9,3)=Nhy+3
   bdc(9,4)=Nhy+4   
   bdc(Nhy+5,1)=-Nhy+1
   bdc(Nhy+6,1)=-Nhy
   bdc(Nhy+7,1)=-Nhy
   bdc(Nhy+8,1)=-Nhy-1
   bdc(Nhy+9,1)=-Nhy-1
   bdc(Nhy+10,1)=-Nhy-2
   bdc(Nhy+11,1)=-Nhy-2
   bdc(Nhy+12,1)=-Nhy-3
   bdc(Nhy+13,1)=-Nhy-4
elseif(iresin.eq.14.or.iresin.eq.15)then !GLU & GLN
   bdc(6,1)=-1
   bdc(6,2)=1
   bdc(6,3)=Nhy-1
   bdc(6,4)=Nhy
   bdc(7,1)=-1
   bdc(7,2)=1
   bdc(7,3)=2
   bdc(8,1)=-1
   bdc(9,1)=-2
   bdc(Nhy+5,1)=-Nhy+1
   bdc(Nhy+6,1)=-Nhy
   if(Iresin.eq.15)then
      bdc(9,2)=Nhy-2
      bdc(9,3)=Nhy-1
      bdc(Nhy+7,1)=-Nhy+2
      bdc(Nhy+8,1)=-Nhy+1
   endif
elseif(Iresin.eq.16)then !ARG
   bdc(6,1)=-1
   bdc(6,2)=1
   bdc(6,3)=Nhy-1
   bdc(6,4)=Nhy
   bdc(7,1)=-1
   bdc(7,2)=1
   bdc(7,3)=Nhy
   bdc(7,4)=Nhy+1
   bdc(8,1)=-1
   bdc(8,2)=1
   bdc(8,3)=Nhy+1
   bdc(9,1)=-1
   bdc(9,2)=1
   bdc(9,3)=2
!   bdc(9,4)=1
   bdc(10,1)=-1
   bdc(10,2)=Nhy
   bdc(10,3)=Nhy+1
!   bdc(10,4)=-1
   bdc(11,1)=-2
   bdc(11,2)=Nhy+1
   bdc(11,3)=Nhy+2
   bdc(Nhy+5,1)=-Nhy+1
   bdc(Nhy+6,1)=-Nhy
   bdc(Nhy+7,1)=-Nhy
   bdc(Nhy+8,1)=-Nhy-1
   bdc(Nhy+9,1)=-Nhy-1
   bdc(Nhy+10,1)=-Nhy
   bdc(Nhy+11,1)=-Nhy-1
   bdc(Nhy+12,1)=-Nhy-1
   bdc(Nhy+13,1)=-Nhy-2
elseif(Iresin.eq.17)then !HIS
   bdc(6,1)=-1
   bdc(6,2)=1
   bdc(6,3)=2
!   bdc(6,4)=2
   bdc(7,1)=-1
   bdc(7,2)=2
   bdc(7,3)=Nhy-2
   bdc(8,1)=-2
!   bdc(8,4)=-2
   bdc(8,2)=2
   bdc(8,3)=Nhy-2
   bdc(9,1)=-2
   bdc(9,2)=1
!   bdc(9,4)=1
   bdc(9,3)=Nhy-2
   bdc(Nhy+5,1)=-Nhy+2
   bdc(Nhy+6,1)=-Nhy+2
   bdc(Nhy+7,1)=-Nhy+2
elseif(Iresin.eq.18.or.Iresin.eq.19)then !PHE and TYR
   bdc(6,1)=-1
   bdc(6,2)=1
!   bdc(6,4)=1
   bdc(6,3)=2
   bdc(7,1)=-1
!   bdc(7,4)=-1
   bdc(7,2)=2
   bdc(7,3)=Nhy-2
   bdc(8,1)=-2
   bdc(8,2)=2
!   bdc(8,4)=2
   bdc(8,3)=Nhy-2
   bdc(9,1)=-2
   bdc(9,2)=2
!   bdc(9,4)=2
   bdc(9,3)=Nhy-2
   bdc(10,1)=-2
!   bdc(10,4)=-2
   bdc(10,2)=1
   bdc(10,3)=Nhy-2
   bdc(11,1)=-2
   bdc(11,2)=-1
!   bdc(11,4)=-2
   bdc(11,3)=Nhy-2
   bdc(Nhy+5,1)=-Nhy+2
   bdc(Nhy+6,1)=-Nhy+2
   bdc(Nhy+7,1)=-Nhy+2
   bdc(Nhy+8,1)=-Nhy+2
   bdc(Nhy+9,1)=-Nhy+2
   if(Iresin.eq.19)then
      bdc(11,3)=1
      bdc(11,4)=-2
      bdc(12,1)=-1
      bdc(12,2)=Nhy-3
      bdc(Nhy+9,1)=-Nhy+3
   endif
elseif(Iresin.eq.20)then !TRP
   bdc(6,1)=-1
   bdc(6,2)=1
!   bdc(6,4)=1
   bdc(6,3)=2
   bdc(7,1)=-1
   bdc(7,2)=2
!   bdc(7,4)=-1
   bdc(7,3)=Nhy-2
   bdc(8,1)=-2
   bdc(8,2)=2
!   bdc(8,4)=2
   bdc(8,3)=3  
   bdc(9,1)=-2
   bdc(9,2)=1
   bdc(9,3)=Nhy-3
   bdc(10,1)=-1
   bdc(10,2)=-2
!   bdc(10,4)=-2
   bdc(10,3)=2
   bdc(11,1)=-3
   bdc(11,2)=2
!   bdc(11,4)=2
   bdc(11,3)=Nhy-4
   bdc(12,1)=-2
   bdc(12,2)=2
!   bdc(12,4)=2
   bdc(12,3)=Nhy-4
   bdc(13,1)=-2
   bdc(13,2)=1
!   bdc(13,4)=-2
   bdc(13,3)=Nhy-4
   bdc(14,1)=-1
   bdc(14,2)=-2
!   bdc(14,4)=-2
   bdc(14,3)=Nhy-4
   bdc(Nhy+5,1)=-Nhy+2
   bdc(Nhy+6,1)=-Nhy+3
   bdc(Nhy+7,1)=-Nhy+4
   bdc(Nhy+8,1)=-Nhy+4
   bdc(Nhy+9,1)=-Nhy+4
   bdc(Nhy+10,1)=-Nhy+4  
endif
end subroutine Assbonda

!!aasign the bond length value to 32*32 matrix
subroutine bondp
  bd0(4,4)=1.335 !C-C
  bd0(23,23)=1.375 !CA-CA
  bd0(4,23)=1.49 !C-CA
  bd0(5,23)=1.49 !CC-CA
  bd0(30,31)=1.527 !CP1-CP2
  bd0(31,31)=1.537 !CP2-CP2
  bd0(31,32)=1.537 !CP2-CP3
  bd0(6,6)=1.36   !CPH1-CPH1
  bd0(8,23)=1.368 !CPT-CA
  bd0(8,8)=1.4 !CPT-CPT
  bd0(1,1)=1.5 !CT1-CT1
  bd0(1,2)=1.538 !CT1-CT2
  bd0(1,3)=1.538 !CT1-CT3
  bd0(1,4)=1.49 !CT1-C
  bd0(1,5)=1.522 !CT1-CC
  bd0(2,2)=1.53 !CT2-CT2
  bd0(2,3)=1.528 !CT2-CT3
  bd0(2,4)=1.49 !CT2-C
  bd0(2,5)=1.522 !CT2-CC
  bd0(2,6)=1.5 !CT2-CPH1
  bd0(2,14)=1.51 !CT2-CY
  bd0(2,23)=1.49 !CT2-CA
  bd0(3,3)=1.53 !CT3-
  bd0(3,4)=1.49 !CT3-C
  bd0(3,23)=1.49 !CT3-CA
  bd0(3,5)=1.522 !CT3-CC
  bd0(3,6)=1.5 !CT3-CPH1
  bd0(14,23)=1.365 !CY-CA
  bd0(8,14)=1.44 !CPT-CY
  bd0(4,30)=1.49
  bd0(5,30)=1.49
!!C finish
  bd0(18,23)=1.083 !HA-CA
  bd0(5,18)=1.1  !HA-CC
  bd0(18,31)=1.111 !HA-CP2
  bd0(18,32)=1.111 !HA-CP3
  bd0(1,18)=1.111
  bd0(2,18)=1.111
  bd0(3,18)=1.111 !HA-CT3
  bd0(14,18)=1.08 !HA-CY
  bd0(19,31)=1.08 !HB-CP1
  bd0(1,19)=1.08
  bd0(2,19)=1.08
  bd0(3,19)=1.08 !HB-CT3
  bd0(23,25)=1.08 !HP-CA
  bd0(14,25)=1.08 !HP-CY
  bd0(6,22)=1.083
  bd0(7,22)=1.09 !HR1-CPH2
  bd0(19,30)=1.08
!!H finish
  bd0(4,9)=1.3  !N-C
  bd0(9,30)=1.434 !N-CP1
  bd0(9,32)=1.455 !N-CP3
  bd0(4,13)=1.365 !NC2-C
  bd0(2,13)=1.49
  bd0(3,13)=1.49 !NC2-CT3
  bd0(13,20)=1.0 !NC2-HC
  bd0(4,10)=1.345
  bd0(1,10)=1.43
  bd0(2,10)=1.43
  bd0(3,10)=1.43 !NH1-CT3
  bd0(10,17)=0.997 !NH1-H
  bd0(10,20)=0.98 !NH1-HC
  bd0(5,11)=1.36 !NH2-CC
  bd0(2,11)=1.455
  bd0(3,11)=1.455 !NH2-CT3
  bd0(1,12)=1.48
  bd0(2,12)=1.48
  bd0(3,12)=1.48 !NH3-CT3
  bd0(12,20)=1.04 !NH3-HC
  bd0(6,15)=1.38 !NR2-CPH1
  bd0(7,15)=1.32
  bd0(16,23)=1.37 !NY-CA
  bd0(8,16)=1.375 !NY-CPT
  bd0(16,17)=0.976 !NY-H
  bd0(11,17)=1.0
  bd0(15,17)=1.0
  bd0(9,27)=1.455
!N finish
  bd0(4,26)=1.23 !O-C
  bd0(5,26)=1.23
  bd0(23,27)=1.26 !OC-CA
  bd0(5,27)=1.26 !OC-CC
  bd0(4,27)=1.26 !OC-CC
  bd0(2,27)=1.33
  bd0(3,27)=1.33 !OC-CT3
  bd0(23,28)=1.411 !OH1-CA
  bd0(1,28)=1.42
  bd0(2,28)=1.42
  bd0(3,28)=1.42 !OH1-CT3
  bd0(17,28)=0.96 !OH1-H
  bd0(2,29)=1.818
  bd0(3,29)=1.816 !S-CT3
  bd0(21,29)=1.325 !HS-S
!1for PRO N_term
  bd0(30,33)=1.485
  bd0(32,33)=1.502
  bd0(20,33)=1.006
end subroutine bondp
!!assign the value of bond angle
subroutine angp
  ag0(23,23,23)=120.0
  ag0(4,9,30)=117.0
  ag0(4,30,31)=112.3
  ag0(5,30,31)=112.3
  ag0(30,31,31)=108.5
  ag0(31,31,32)=108.5
  ag0(4,9,32)=117.0
  ag0(30,9,32)=114.2
  ag0(6,15,7)=104.0
  ag0(8,23,23)=118.0
  ag0(8,8,23)=122.0
  ag0(8,14,23)=107.4
  ag0(8,16,23)=108.0
  ag0(1,1,4)=108.0
  ag0(1,1,5)=108.0
  ag0(1,1,1)=111.0
  ag0(1,1,2)=111.0
  ag0(1,1,3)=108.5
  ag0(1,2,1)=113.5
  ag0(1,2,2)=113.5
  ag0(1,2,3)=113.5
  ag0(1,2,5)=108.0
  ag0(1,2,6)=113.0
  ag0(1,2,14)=114.0
  ag0(1,2,23)=107.5
  ag0(1,10,4)=120.0
  ag0(2,23,23)=122.3
  ag0(2,6,6)=130.0
  ag0(2,1,3)=114.0
  ag0(2,1,4)=108.0
  ag0(2,1,5)=108.0
  ag0(2,2,4)=108.0
  ag0(2,2,5)=108.0
  ag0(2,2,2)=113.5
  ag0(2,2,3)=115.0
  ag0(2,2,14)=114.0
  ag0(2,14,23)=129.4
  ag0(2,14,8)=124.0
  ag0(2,13,4)=120.0
  ag0(2,10,4)=120.0
  ag0(2,29,3)=95.0
  ag0(3,23,23)=122.3
  ag0(3,6,6)=130.0
  ag0(3,1,3)=114.0
  ag0(3,1,4)=108.0
  ag0(3,1,5)=108.0
  ag0(3,1,23)=107.5
  ag0(3,2,23)=107.5
  ag0(3,2,6)=113.0
  ag0(3,2,6)=114.0
  ag0(3,13,4)=120.0
  ag0(3,10,4)=109.6
  ag0(8,8,14)=107.4
  ag0(14,8,23)=130.0
  ag0(4,10,17)=123.0
  ag0(1,10,17)=117.0
  ag0(2,10,17)=117.0
  ag0(3,10,17)=117.0
  ag0(5,10,17)=120.0
  ag0(17,10,17)=120.0
  ag0(17,16,23)=126.0
  ag0(8,16,17)=126.0
  ag0(17,28,23)=108.0
  ag0(1,28,17)=106.0
  ag0(2,28,17)=106.0
  ag0(3,28,17)=106.0
  ag0(18,23,23)=120.0
  ag0(8,23,18)=122.0
  ag0(14,23,18)=125.0
  ag0(18,31,30)=110.0
  ag0(18,31,31)=110.0
  ag0(18,31,32)=110.0
  ag0(18,31,18)=109.0
  ag0(18,32,31)=110.0
  ag0(18,32,18)=109.0
  ag0(4,1,18)=109.5
  ag0(1,1,18)=110.0
  ag0(2,1,18)=110.0
  ag0(3,1,18)=110.0
  ag0(18,1,18)=109.0
  ag0(4,2,18)=109.5
  ag0(5,2,18)=109.5
  ag0(6,2,18)=109.5
  ag0(18,2,23)=107.5
  ag0(1,2,18)=110.0
  ag0(2,2,18)=110.0
  ag0(3,2,18)=110.0
  ag0(14,2,18)=109.5
  ag0(18,2,18)=109.5
  ag0(4,3,18)=109.5
  ag0(5,3,18)=109.5
  ag0(6,3,18)=109.5
  ag0(18,3,23)=107.5
  ag0(1,3,18)=110.0
  ag0(2,3,18)=110.0
  ag0(3,3,18)=110.0
  ag0(18,3,18)=109.4
  ag0(18,16,23)=126.4
  ag0(8,16,18)=126.4
  ag0(4,30,19)=112.0 
  ag0(5,30,19)=112.0
  ag0(4,1,19)=109.5
  ag0(5,1,19)=109.5
  ag0(1,1,19)=111.0
  ag0(2,1,19)=111.0
  ag0(3,1,19)=111.0
  ag0(4,2,19)=109.5
  ag0(5,2,19)=109.5
  ag0(19,2,19)=115.0
  ag0(4,3,19)=109.5
  ag0(2,13,20)=120.0
  ag0(3,13,20)=120.0
  ag0(4,13,20)=120.0
  ag0(20,13,20)=120.0
  ag0(2,11,20)=111.0
  ag0(3,11,20)=111.0
  ag0(20,11,20)=106.5
  ag0(1,12,20)=109.5
  ag0(2,12,20)=109.5
  ag0(3,12,20)=109.5
  ag0(20,12,20)=109.5
  ag0(23,12,25)=120.0
  ag0(8,12,25)=122.0
  ag0(14,12,25)=125.0
  ag0(23,14,25)=126.4
  ag0(8,14,25)=126.4
  ag0(6,6,22)=130.0
  ag0(6,6,24)=130.0
  ag0(5,11,17)=120.0
  ag0(7,15,17)=126.0
  ag0(15,7,22)=125.0
  ag0(23,23,25)=120.0
  ag0(2,29,21)=95.0  
  ag0(3,29,21)=95.0  !!H finish
  ag0(9,4,30)=112.5  
  ag0(1,4,9)=112.5 
  ag0(2,4,9)=112.5 
  ag0(3,4,9)=112.5 
  ag0(4,30,9)=108.2 
  ag0(5,30,9)=108.2 
  ag0(9,30,31)=110.8 
  ag0(9,30,19)=112.0 
  ag0(9,32,18)=108.0 
  ag0(9,32,31)=110.5 
  ag0(13,4,13)=120.0 
  ag0(2,2,13)=107.5
  ag0(13,2,18)=107.5 
  ag0(13,3,18)=107.5
  ag0(10,4,30)=116.5
  ag0(1,4,10)=116.5
  ag0(2,4,10)=116.5
  ag0(3,4,10)=116.5
  ag0(1,1,10)=113.5
  ag0(2,1,10)=113.5
  ag0(3,1,10)=113.5
  ag0(4,1,10)=107.0
  ag0(5,1,10)=107.0
  ag0(10,1,19)=108.0
  ag0(2,2,10)=113.5
  ag0(4,2,10)=107.0
  ag0(5,2,10)=107.0
  ag0(10,2,18)=109.5
  ag0(10,2,19)=108.0
  ag0(10,3,18)=109.5
  ag0(11,5,30)=112.5
  ag0(1,5,11)=116.5
  ag0(2,5,11)=116.5
  ag0(3,5,11)=116.5
  ag0(11,5,18)=111.0
  ag0(11,2,18)=109.5
  ag0(11,2,19)=109.5
  ag0(2,2,11)=110.0
  ag0(11,3,18)=109.5
  ag0(1,1,12)=110.0
  ag0(2,1,12)=110.0
  ag0(3,1,12)=110.0
  ag0(4,1,12)=110.0
  ag0(5,1,12)=110.0
  ag0(12,1,19)=107.5
  ag0(4,2,12)=110.0
  ag0(5,2,12)=110.0
  ag0(2,2,12)=110.0
  ag0(12,2,18)=107.5
  ag0(12,2,19)=107.5
  ag0(12,3,18)=107.5
  ag0(6,6,15)=110.0
  ag0(2,6,15)=120.0
  ag0(15,6,24)=120.0
  ag0(15,6,22)=125.0
  ag0(14,23,16)=110.0
  ag0(16,23,18)=125.0
  ag0(16,23,25)=125.0
  ag0(16,8,23)=130.6
  ag0(8,8,16)=107.4  !!N finish
  ag0(26,4,30)=118.0
  ag0(1,4,26)=121.0
  ag0(2,4,26)=121.0
  ag0(3,4,26)=121.0
  ag0(17,4,26)=121.0
  ag0(9,4,26)=122.5
  ag0(10,4,26)=122.5
  ag0(26,5,30)=118.0
  ag0(1,5,26)=121.0
  ag0(2,5,26)=121.0
  ag0(3,5,26)=121.0
  ag0(18,5,26)=122.0
  ag0(11,5,26)=122.5
  ag0(23,23,27)=120.0
  ag0(27,5,30)=118.0
  ag0(1,5,27)=118.0
  ag0(2,5,27)=118.0
  ag0(3,5,27)=118.0
  ag0(27,5,27)=124.0
  ag0(3,2,27)=122.0
  ag0(18,2,27)=118.3
  ag0(19,2,27)=118.3
  ag0(23,23,28)=120.0
  ag0(1,1,28)=110.1
  ag0(3,1,28)=110.1
  ag0(18,1,28)=108.89
  ag0(1,2,28)=110.1
  ag0(2,2,28)=110.1
  ag0(3,2,28)=110.1
  ag0(26,5,27)=124.0
  ag0(18,2,28)=108.89
  ag0(18,3,28)=108.89 !!O finish  
  ag0(1,2,29)=112.5
  ag0(2,2,29)=114.5
  ag0(3,2,29)=114.5
  ag0(18,2,29)=111.3
  ag0(18,3,29)=111.3
!!for PRO N_term
  ag0(30,33,32)=111.0
  ag0(17,33,31)=109.5
  ag0(17,33,33)=109.5
  ag0(17,33,17)=107.5
  ag0(4,30,33)=106.0
  ag0(5,30,33)=106.0
  ag0(32,30,33)=108.5
  ag0(19,30,33)=107.5
  ag0(30,32,33)=108.5
  ag0(18,32,33)=109.15
  ag0(20,33,32)=109.5
end subroutine angp

end program main